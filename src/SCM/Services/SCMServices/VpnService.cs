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
            IRouteTargetService routeTargetService, IAttachmentOrVifService attachmentOrVifService, 
            INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
            RouteTargetService = routeTargetService;
            AttachmentOrVifService = attachmentOrVifService;
        }

        private IRouteTargetService RouteTargetService { get; set; }
        private IAttachmentOrVifService AttachmentOrVifService { get; set; }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties: "Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant,Region");
        }

        public async Task<Vpn> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnRepository.GetByIDAsync(key);
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

            var syncResult = await NetSync.DeleteFromNetworkAsync("/ip-vpn/vpn/" + vpn.Name);

            if (syncResult.HttpStatusCode != HttpStatusCode.NotFound)
            {
                serviceResult.IsSuccess = false;
                serviceResult.AddRange(syncResult.Messages);

                return serviceResult;
            }     

            this.UnitOfWork.VpnRepository.Delete(vpn);
            await this.UnitOfWork.SaveAsync();

            return serviceResult;
        }

        /// <summary>
        /// Validate a new VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateAsync(Vpn vpn)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            if (vpn.IsExtranet)
            {
                var vpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

                if (vpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("Extranet is supported only for Hub-and-Spoke IP VPN topologies.");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Validate changes to a VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <param name="currentVpn"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

            if (currentVpn == null)
            {
                validationResult.Add("Unable to save changes. The VPN was deleted by another user.");
                validationResult.IsSuccess = false;

                return validationResult;
            }

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
                if  (distinctRegionsCount == 1)
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

        public async Task<NetworkSyncServiceResult> CheckNetworkSyncAsync(int vpnID)
        {
            var checkSyncResult = new NetworkSyncServiceResult();

            var vpnDbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID,
                includeProperties: "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device,"
                    + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                    + "VpnTopologyType,RouteTargets");

            var vpn = vpnDbResult.SingleOrDefault();
            if (vpn == null)
            {
                checkSyncResult.Messages.Add("The VPN was not found.");
            }
            else
            {
                var validationResult = await ValidateVpn(vpn);
                if (!validationResult.IsSuccess)
                {
                    checkSyncResult.Messages.Add(validationResult.GetMessage());

                    return checkSyncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                checkSyncResult = await NetSync.CheckNetworkSyncAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

            await UpdateVpnRequiresSyncAsync(vpn, !checkSyncResult.IsSuccess);

            return checkSyncResult;
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(int vpnID)
        {
            var syncResult = new NetworkSyncServiceResult { IsSuccess = true };
 
            var vpnDbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID,
                includeProperties: "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device,"
                    + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                    + "VpnTopologyType,RouteTargets");

            var vpn = vpnDbResult.SingleOrDefault();
            if (vpn == null)
            {
                syncResult.Messages.Add("The VPN was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                var validationResult = await ValidateVpn(vpn);
                if (!validationResult.IsSuccess)
                {
                    syncResult.Messages.Add(validationResult.GetMessage());
                    syncResult.IsSuccess = false;

                    return syncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                syncResult = await NetSync.SyncNetworkAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

            await UpdateVpnRequiresSyncAsync(vpn, !syncResult.IsSuccess);

            return syncResult;
        }

        public async Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int vpnID)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var dbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID, AsTrackable: false);
            var vpn = dbResult.SingleOrDefault();

            if (vpn == null)
            {
                syncResult.Messages.Add("The VPN was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                syncResult = await NetSync.DeleteFromNetworkAsync("/ip-vpn/vpn/" + vpn.Name);
            }

            await UpdateVpnRequiresSyncAsync(vpn, true);

            return syncResult;
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
            var vpn = await GetByIDAsync(vpnID);
            await UpdateVpnRequiresSyncAsync(vpn, requiresSync, saveChanges);

            return;
        }

        /// <summary>
        /// Validate a VPN before syncing to the network
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        private async Task<ServiceResult> ValidateVpn(Vpn vpn)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            if (vpn.VpnTopologyType.TopologyType == "Any-to-Any")
            {
                if (vpn.RouteTargets.Count() != 1)
                {
                    validationResult.IsSuccess = false;
                    validationResult.Add("Route targets must be correctly defined for the VPN.");
                }
            }
            else
            {
                if (vpn.RouteTargets.Count() != 2)
                {
                    validationResult.IsSuccess = false;
                    validationResult.Add("Route targets must be correctly defined for the VPN.");
                }
            }

            var attachmentsAndVifs = await AttachmentOrVifService.GetAllByVpnIDAsync(vpn.VpnID);
            if (attachmentsAndVifs.Where(q => q.RequiresSync == true).Count() > 0)
            {
                validationResult.IsSuccess = false;
                validationResult.Add("One or more attachments or vifs for the VPN require synchronisation with the network.");
                validationResult.Add("Synchronise the attachments or vifs first and then try again.");
            }

            return validationResult;
        }
    }
}