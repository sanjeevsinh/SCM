using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels.IpVpnNetModels;
using System.Threading.Tasks;
using AutoMapper;
using System.Net;

namespace SCM.Services.SCMServices
{
    public class VpnService : BaseService, IVpnService
    {
        public VpnService(IUnitOfWork unitOfWork, IMapper mapper,
            IRouteTargetService routeTargetService,
            IAttachmentService attachmentService,
            IVifService vifService,
            INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
            RouteTargetService = routeTargetService;
            AttachmentService = attachmentService;
            VifService = vifService;
        }

        private IRouteTargetService RouteTargetService { get; set; }
        private IAttachmentService AttachmentService { get; set; }
        private IVifService VifService { get; set; }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties: "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType," 
                + "Tenant," 
                + "Region");
        }

        public async Task<Vpn> GetByIDAsync(int id)
        {
            var dbResult = await this.UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == id, 
                includeProperties: "Region,"
                + "Plane,"
                + "VpnTenancyType,"
                + "VpnTopologyType.VpnProtocolType,"
                + "Tenant,"
                + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,"
                + "VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                + "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device,"
                + "RouteTargets", AsTrackable: false);

            return dbResult.SingleOrDefault();
        }

        public async Task<ServiceResult> AddAsync(Vpn vpn)
        {

            var result = new ServiceResult { IsSuccess = true };
            var vpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

            // Get some route targets

            var routeTargets = await RouteTargetService.AllocateAllVpnRouteTargetsAsync(vpnTopologyType.TopologyType, result);

            // Quit if failed to allocate route targets

            if (!result.IsSuccess)
            {
                return result;
            }

            try
            {
                // Implement transactionScope here when available in dotnet core

                vpn.RequiresSync = true;
                this.UnitOfWork.VpnRepository.Insert(vpn);

                // Save in order to generat a new vpn ID

                await this.UnitOfWork.SaveAsync();

                foreach (RouteTarget rt in routeTargets)
                {
                    rt.VpnID = vpn.VpnID;
                    this.UnitOfWork.RouteTargetRepository.Insert(rt);
                }

                await this.UnitOfWork.SaveAsync();
            }

            catch
            {
                // Add logging for the exception here
                result.Add("Something went wrong during the database update. The issue has been logged."
                   + "Please try again, and contact your system admin if the problem persists.");
                result.IsSuccess = false;
            }

            return result;
        }

        public async Task<int> UpdateAsync(Vpn vpn)
        {
            vpn.RequiresSync = true;
            this.UnitOfWork.VpnRepository.Update(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> DeleteAsync(Vpn vpn)
        {
            var serviceResult = new ServiceResult { IsSuccess = true };

            this.UnitOfWork.VpnRepository.Delete(vpn);
            await this.UnitOfWork.SaveAsync();

            return serviceResult;
        }

        /// <summary>
        /// Validate a new VPN request
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync(Vpn vpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var topologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

            if (vpn.IsExtranet)
            {
                if (topologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("Extranet is supported only for Hub-and-Spoke IP VPN topologies.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate an existing VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(Vpn vpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var attachments = await AttachmentService.GetAllByVpnIDAsync(vpn.VpnID);
            if (attachments.Where(q => q.RequiresSync).Count() > 0)
            {
                validationResult.IsSuccess = false;
                validationResult.Add("One or more attachments for the VPN require synchronisation with the network.");
            }

            var vifs = await VifService.GetAllByVpnIDAsync(vpn.VpnID);
            if (vifs.Where(q => q.RequiresSync).Count() > 0)
            {
                validationResult.IsSuccess = false;
                validationResult.Add("One or more vifs for the VPN require synchronisation with the network.");
            }

            return validationResult;
        }

        /// <summary>
        /// Validate change requests to a VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="currentVpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var vpnTenancyType = await UnitOfWork.VpnTenancyTypeRepository.GetByIDAsync(vpn.VpnTenancyTypeID);
            if (vpnTenancyType.TenancyType == "Single" && currentVpn.VpnTenancyType.TenancyType == "Multi")
            {
                // The tenancy type can be narrowed only if the only tenant of the VPN is the owner.

                var tenants = currentVpn.VpnAttachmentSets.Select(s => s.AttachmentSet.Tenant);
                var ownerCount = tenants.Count(s => s.Name == currentVpn.Tenant.Name);

                if (ownerCount != tenants.Count())
                {
                    validationResult.Add("The Tenancy Type cannot be changed from Multi to Single because tenants other "
                        + "than the owner are attached to the VPN.");
                    validationResult.IsSuccess = false;
                }
            }

            // Check if Region has been narrowed to a specific region, or changed from one region to another

            if (vpn.RegionID != null && (currentVpn.RegionID == null || vpn.RegionID != currentVpn.RegionID))
            {
                // Region can be narrowed or changed only if the only devices with VRFs which participate in the VPN are in the 
                // selected region.

                var regions = currentVpn.VpnAttachmentSets.SelectMany(q =>
                q.AttachmentSet.AttachmentSetVrfs.Select(r => r.Vrf.Device.Location.SubRegion.Region));

                var distinctRegionsCount = regions.Distinct().Count();
                if (distinctRegionsCount == 1)
                {
                    var region = regions.Distinct().Single();
                    if (region.RegionID != vpn.RegionID)
                    {
                        validationResult.Add("The Region setting cannot be narrowed to a specific region because all of the tenants "
                            + "of the VPN exist in the " + region.Name + " region.");
                        validationResult.IsSuccess = false;
                    }
                }
                else if (distinctRegionsCount > 1)
                {
                    validationResult.Add("The Region setting cannot be narrowed to a specific region because tenants of the VPN "
                        + "exist in more than one region.");
                    validationResult.IsSuccess = false;
                }
            }

            if (vpn.IsExtranet)
            {
                var VpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);
                if (VpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("The Extranet attribute can only be set for VPNs with the 'Hub-and-Spoke' topology.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }

        public async Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn)
        {
            var result = new ServiceResult();

            var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
            var syncResult = await NetSync.CheckNetworkSyncAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;

            await UpdateVpnRequiresSyncAsync(vpn, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> SyncToNetworkAsync(Vpn vpn)
        {
            var result = new ServiceResult();

            var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
            var syncResult = await NetSync.SyncNetworkAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);

            result.AddRange(syncResult.Messages);
            result.IsSuccess = syncResult.IsSuccess;
       
            await UpdateVpnRequiresSyncAsync(vpn, !result.IsSuccess);

            return result;
        }

        public async Task<ServiceResult> DeleteFromNetworkAsync(Vpn vpn)
        {
            var result = new ServiceResult();

            var syncResult = await NetSync.DeleteFromNetworkAsync("/ip-vpn/vpn/" + vpn.Name);
            result.NetworkSyncServiceResults.Add(syncResult);
            result.IsSuccess = syncResult.IsSuccess;

            await UpdateVpnRequiresSyncAsync(vpn, true);

            return result;
        }

        /// <summary>
        /// Update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateVpnRequiresSyncAsync(Vpn vpn, bool requiresSync, bool saveChanges = true)
        {
            vpn.RequiresSync = requiresSync;
            UnitOfWork.VpnRepository.Update(vpn);
            if (saveChanges)
            {
                await UnitOfWork.SaveAsync();
            }

            return;
        }

        /// <summary>
        /// Update the RequiresSync property of a vpn record.
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="requiresSync"></param>
        /// <returns></returns>
        public async Task UpdateVpnRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges = true)
        {
            var vpn = await UnitOfWork.VpnRepository.GetByIDAsync(vpnID);
            await UpdateVpnRequiresSyncAsync(vpn, requiresSync, saveChanges);

            return;
        }
    }
}