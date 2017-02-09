using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class RouteTargetService : BaseService, IRouteTargetService
    {
        public RouteTargetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<RouteTarget>> GetAllAsync()
        {
            return await this.UnitOfWork.RouteTargetRepository.GetAsync(includeProperties: "Plane,RouteTargetTenancyType,RouteTargetTopologyType.RouteTargetProtocolType,Tenant,Region");
        }

        public async Task<RouteTarget> GetByIDAsync(int key)
        {
            return await UnitOfWork.RouteTargetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Insert(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Update(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(RouteTarget routeTarget)
        {
            this.UnitOfWork.RouteTargetRepository.Delete(routeTarget);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceValidationResult> ValidateRouteTargetChangesAsync(RouteTarget routeTarget)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            var vpnAttachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == routeTarget.VpnID, 
                includeProperties:"AttachmentSet", AsTrackable:false);

            if (vpnAttachmentSets.Count > 0)
            {
                validationResult.Add("A Route Target cannot be added, deleted, or changed because the following Attachment Sets are bound to the VPN:");
                validationResult.Add(string.Join(",", vpnAttachmentSets.Select(a => a.AttachmentSet.Name).ToArray()) + ". ");
                validationResult.IsValid = false;
            }

            return validationResult;
        }

        /// <summary>
        /// Validate route targets are correctly defined for the current vpn.
        /// </summary>
        /// <param name="vpnID"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateRouteTargetsAsync(int vpnID)
        {
            var dbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, includeProperties: "VpnTopologyType.VpnProtocolType,RouteTargets");
            var vpn = dbResult.SingleOrDefault();
            var serviceValidationData = new ServiceValidationResult();

            if (vpn == null)
            {
                serviceValidationData.Add("The VPN was not found.");
                return serviceValidationData;
            }
            else
            {
                var protocolType = vpn.VpnTopologyType.VpnProtocolType.ProtocolType;
                var topologyType = vpn.VpnTopologyType.TopologyType;
                var countOfRouteTargets = vpn.RouteTargets.Count();
                var countOfExportRouteTarget = vpn.RouteTargets.Where(r => r.IsHubExport == true).Count();

                serviceValidationData.IsValid = true;

                if (protocolType == "Ethernet")
                {
                    if (countOfExportRouteTarget > 0)
                    {
                        serviceValidationData.Add("A Hub Export route target cannot be defined for Ethernet VPN types.");
                        serviceValidationData.IsValid = false;
                    }
                }
                else
                {
                    if (topologyType == "Any-to-Any")
                    {
                        if (countOfRouteTargets != 1)
                        {
                            serviceValidationData.Add("Any-to-Any IP VPNs require one route target.");
                            serviceValidationData.IsValid = false;
                        }

                        if (countOfExportRouteTarget > 0)
                        {
                            serviceValidationData.Add("Hub Export cannot be set for Any-to-Any IP VPN types.");
                            serviceValidationData.IsValid = false;
                        }
                    }
                    else if (topologyType == "Hub-and-Spoke")
                    {
                        if (countOfRouteTargets != 2)
                        {
                            serviceValidationData.Add("Hub-and-Spoke IP VPNs require two route targets, one of which must be an export route target.");
                            serviceValidationData.IsValid = false;
                        }


                        if (countOfExportRouteTarget != 1)
                        {
                            serviceValidationData.Add("Hub-and-Spoke IP VPNs require one export route target.");
                            serviceValidationData.IsValid = false;
                        }
                    }
                }
            }

            return serviceValidationData;
        }
    }
}