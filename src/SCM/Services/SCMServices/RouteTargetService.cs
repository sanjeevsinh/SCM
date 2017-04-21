using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;
using SCM.Models.ServiceModels;
using AutoMapper;

namespace SCM.Services.SCMServices
{
    public class RouteTargetService : BaseService, IRouteTargetService
    {
        public RouteTargetService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            UnitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RouteTarget>> GetAllAsync()
        {
            return await this.UnitOfWork.RouteTargetRepository.GetAsync(includeProperties: 
                "Plane,"
                + "RouteTargetTenancyType,"
                + "RouteTargetTopologyType.RouteTargetProtocolType,"
                + "Tenant,"
                + "Region");
        }

        public async Task<RouteTarget> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetID == id, includeProperties: "Vpn", AsTrackable: false);
            return dbResult.SingleOrDefault();
        }
   
        public async Task<IEnumerable<RouteTarget>> GetAllByVpnIDAsync(int id)
        {
            return await UnitOfWork.RouteTargetRepository.GetAsync(q => q.VpnID == id, includeProperties: "Vpn", AsTrackable: false);
        }

        public async Task<ServiceResult> AddAsync(RouteTargetRequest request)
        {
            var result = new ServiceResult { IsSuccess = true };

            await AllocateRouteTarget(request, result);
            if (!result.IsSuccess)
            {
                return result;
            }

            var routeTarget = Mapper.Map<RouteTarget>(request);
            this.UnitOfWork.RouteTargetRepository.Insert(routeTarget);

            // RTs have changed - must flag VPN as needing sync with network

            await UpdateVpnRequiresSyncAsync(routeTarget.VpnID, true, false);
            await this.UnitOfWork.SaveAsync();

            return result;
        }

        public async Task<int> DeleteAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Delete(routeTarget);

            // RTs have changed - must flag VPN as needing sync with network

            await UpdateVpnRequiresSyncAsync(routeTarget.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate an existing VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public ServiceResult Validate(Vpn vpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var protocolType = vpn.VpnTopologyType.VpnProtocolType.ProtocolType;
            var topologyType = vpn.VpnTopologyType.TopologyType;
            var countOfRouteTargets = vpn.RouteTargets.Count();
            var countOfExportRouteTarget = vpn.RouteTargets.Where(r => r.IsHubExport == true).Count();

            if (protocolType == "Ethernet")
            {
                if (countOfExportRouteTarget > 0)
                {
                    validationResult.Add("A hub export route target cannot be defined for Ethernet VPN types.");
                    validationResult.IsSuccess = false;
                }
            }
            else
            {
                if (topologyType == "Any-to-Any")
                {
                    if (countOfRouteTargets != 1)
                    {
                        validationResult.Add("Any-to-Any IP VPNs require one route target.");
                        validationResult.IsSuccess = false;
                    }

                    if (countOfExportRouteTarget > 0)
                    {
                        validationResult.Add("Hub Export cannot be set for Any-to-Any IP VPN types.");
                        validationResult.IsSuccess = false;
                    }
                }
                else if (topologyType == "Hub-and-Spoke")
                {
                    if (countOfRouteTargets != 2)
                    {
                        validationResult.Add("Hub-and-Spoke IP VPNs require two route targets, one of which must be an export route target.");
                        validationResult.IsSuccess = false;
                    }


                    if (countOfExportRouteTarget != 1)
                    {
                        validationResult.Add("Hub-and-Spoke IP VPNs require one export route target.");
                        validationResult.IsSuccess = false;
                    }
                }
            }

            return validationResult;
        }

        public async Task<ServiceResult> CheckVpnOkToAddOrRemoveRouteTargetAsync(int vpnID)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == vpnID,
                includeProperties: "AttachmentSet", AsTrackable: false);

            if (vpnAttachmentSets.Count > 0)
            {
                validationResult.Add("Route targets cannot be changed because the following attachment sets are bound to the VPN:");
                validationResult.Add(string.Join(",", vpnAttachmentSets.Select(a => a.AttachmentSet.Name).ToArray()) + ". ");
                validationResult.IsSuccess = false;
            }

            return validationResult;
        }

        public async Task<IEnumerable<RouteTarget>> AllocateAllVpnRouteTargetsAsync(string vpnTopologyType, ServiceResult result)
        {

            var rtRangeResult = await UnitOfWork.RouteTargetRangeRepository.GetAsync(q => q.Name == "Default");
            var rtRange = rtRangeResult.SingleOrDefault();

            if (rtRange == null)
            {
                result.Add("The default route target range was not found.");
                result.IsSuccess = false;
            }

            var rtAssignedNumbers = await FindRouteTargetAssignedNumbersAsync(vpnTopologyType, rtRange, result);

            var rts = new List<RouteTarget>();
            for (int i = 0; i <= rtAssignedNumbers.Count() -1 ; i++) {
                
                rts.Add(new RouteTarget
                {
                    AdministratorSubField = rtRange.AdministratorSubField,
                    AssignedNumberSubField = rtAssignedNumbers.ToList()[i],
                    RouteTargetRangeID = rtRange.RouteTargetRangeID,
                    IsHubExport = vpnTopologyType == "Hub-and-Spoke" && i == 1 
                });
            }

            return rts;
        }

        /// <summary>
        /// Find free assigned numbers for route targets
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task<IEnumerable<int>> FindRouteTargetAssignedNumbersAsync(string topologyType, RouteTargetRange rtRange, ServiceResult result)
        {

            var usedRTs = await UnitOfWork.RouteTargetRepository.GetAsync(q => q.RouteTargetRangeID == rtRange.RouteTargetRangeID);

            // Allocate new unused RTs from the RT range

            IEnumerable<int> rtAssignedNumbers = null;

            if (topologyType == "Any-to-Any")
            {
                rtAssignedNumbers = Enumerable.Range(rtRange.AssignedNumberSubFieldStart, rtRange.AssignedNumberSubFieldCount)
                    .Except(usedRTs.Select(q => q.AssignedNumberSubField)).Take(1);

                if (rtAssignedNumbers.Count() != 1)
                {
                    result.Add("Failed to allocate a free route target. Please contact your administrator, or try another range.");
                    result.IsSuccess = false;

                    return Enumerable.Empty<int>();
                }
            }
            else if (topologyType == "Hub-and-Spoke")
            {
                rtAssignedNumbers = Enumerable.Range(rtRange.AssignedNumberSubFieldStart, rtRange.AssignedNumberSubFieldCount)
                    .Except(usedRTs.Select(q => q.AssignedNumberSubField)).Take(2);

                if (rtAssignedNumbers.Count() != 2)
                {
                    result.Add("Failed to allocate two free route targets. Please contact your administrator, or try another range.");
                    result.IsSuccess = false;

                    return Enumerable.Empty<int>();
                }
            }

            return rtAssignedNumbers;
        }

        /// <summary>
        /// Helper to allocate a single route target
        /// </summary>
        /// <param name="request"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task AllocateRouteTarget(RouteTargetRequest request, ServiceResult result)
        {
            if (request.AutoAllocateAssignedNumberSubField)
            {
                var dbResult = await UnitOfWork.RouteTargetRangeRepository.GetAsync(q => q.Name == "Default");
                var rtRange = dbResult.SingleOrDefault();

                if (rtRange == null)
                {
                    result.Add("The default route target range was not found.");
                    result.IsSuccess = false;

                    return;
                }

                var currentRts = await UnitOfWork.RouteTargetRepository.GetAsync(q => q.AdministratorSubField == request.AdministratorSubField);

                // Allocate a new unused rt from the rt range

                int? newAssignedNumber = Enumerable.Range(rtRange.AssignedNumberSubFieldStart, rtRange.AssignedNumberSubFieldCount)
                    .Except(currentRts.Select(rt => rt.AssignedNumberSubField)).FirstOrDefault();

                if (newAssignedNumber == null)
                {
                    result.Add("Failed to allocate a free route target. Please contact your administrator, or try another range.");
                    result.IsSuccess = false;

                    return;
                }

                request.RouteTargetRangeID = rtRange.RouteTargetRangeID;
                request.AllocatedAssignedNumberSubField = newAssignedNumber.Value;
            }
            else
            {
                if (request.RequestedAssignedNumberSubField == null)
                {
                    result.Add("A requested assigned-number sub-field value must be specified, or select the auto-allocate option.");
                    result.IsSuccess = false;

                    return;
                }

                var rtQuery = await UnitOfWork.RouteTargetRepository.GetAsync(q => q.AdministratorSubField == request.AdministratorSubField 
                    && q.AssignedNumberSubField == request.RequestedAssignedNumberSubField, includeProperties:"Vpn");
                var rt = rtQuery.SingleOrDefault();

                if (rt != null)
                {
                    result.Add($"The request route target is already in use for vpn '{rt.Vpn.Name}'.");
                    result.Add("Try again with a different route target.");
                    result.IsSuccess = false;

                    return;
                }
                else
                {
                    request.AllocatedAssignedNumberSubField = request.RequestedAssignedNumberSubField.Value;

                    return;
                }
            }
        }
        /// <summary>
        /// Update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        private async Task UpdateVpnRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges = true)
        {
            var vpn = await UnitOfWork.VpnRepository.GetByIDAsync(vpnID);
            vpn.RequiresSync = requiresSync;
            UnitOfWork.VpnRepository.Update(vpn);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }
    }
}