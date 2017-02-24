using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels.IpVpn;
using System.Threading.Tasks;
using AutoMapper;
using System.Net;

namespace SCM.Services.SCMServices
{
    public class VpnService : BaseService, IVpnService
    {
        public VpnService(IUnitOfWork unitOfWork, IMapper mapper, INetworkSyncService netSync) : base(unitOfWork, mapper, netSync)
        {
        }

        public async Task<IEnumerable<Vpn>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnRepository.GetAsync(includeProperties: "Plane,VpnTenancyType,VpnTopologyType.VpnProtocolType,Tenant,Region");
        }

        public async Task<Vpn> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Insert(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Update(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<ServiceResult> DeleteAsync(Vpn vpn)
        {
            var serviceResult = new ServiceResult();
            serviceResult.IsSuccess = true;

            var syncResult = await DeleteFromNetworkAsync(vpn.VpnID);

            if (!syncResult.IsSuccess && syncResult.NetworkHttpResponse.HttpStatusCode != HttpStatusCode.NotFound)
            {
                serviceResult.IsSuccess = false;
                serviceResult.Add(syncResult.GetAllMessages());
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
        public async Task<ServiceResult> ValidateCreateVpnAsync(Vpn vpn)
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
        public async Task<ServiceResult> ValidateVpnChangesAsync(Vpn vpn, Vpn currentVpn)
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
                // Region can be narrowed or changed only if the only devices with VRFs which participate in the VRF are in the 
                // required region.

                var regions = currentVpn.VpnAttachmentSets.SelectMany(q =>
                q.AttachmentSet.AttachmentSetVrfs.Select(r => r.Vrf.Device.Location.SubRegion.Region));

                if (regions.Distinct().Count() == 1)
                {
                    var region = regions.Distinct().Single();
                    if (region.RegionID != vpn.RegionID)
                    {

                        validationResult.Add("The Region setting cannot be narrowed to a specific region because all of the tenants "
                            + "of the VPN exist in the " + region.Name + " region.");
                        validationResult.IsSuccess = false;
                    }
                }
                else
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

        public async Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(int vpnID)
        {
            var checkSyncResult = new NetworkCheckSyncServiceResult();

            var vpnDbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID,
                includeProperties: "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device,"
                    + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                    + "VpnTopologyType,RouteTargets");

            var vpn = vpnDbResult.SingleOrDefault();
            if (vpn == null)
            {
                checkSyncResult.NetworkSyncServiceResult.Add("The VPN was not found.");
            }
            else
            {
                var validationResult = ValidateVpn(vpn);
                if (!validationResult.IsSuccess)
                {
                    checkSyncResult.NetworkSyncServiceResult.Add(validationResult.GetMessage());

                    return checkSyncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                checkSyncResult = await NetSync.CheckNetworkSyncAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

            return checkSyncResult;
        }

        public async Task<NetworkSyncServiceResult> SyncToNetworkAsync(int vpnID)
        {
            var syncResult = new NetworkSyncServiceResult();
            syncResult.IsSuccess = true;

            var vpnDbResult = await UnitOfWork.VpnRepository.GetAsync(q => q.VpnID == vpnID,
                includeProperties: "VpnAttachmentSets.AttachmentSet.AttachmentSetVrfs.Vrf.Device,"
                    + "VpnAttachmentSets.VpnTenantNetworks.TenantNetwork,VpnAttachmentSets.VpnTenantCommunities.TenantCommunity,"
                    + "VpnTopologyType,RouteTargets");

            var vpn = vpnDbResult.SingleOrDefault();
            if (vpn == null)
            {
                syncResult.Add("The VPN was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                var validationResult = ValidateVpn(vpn);
                if (!validationResult.IsSuccess)
                {
                    syncResult.Add(validationResult.GetMessage());
                    syncResult.IsSuccess = false;

                    return syncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                syncResult = await NetSync.SyncNetworkAsync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

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
                syncResult.Add("The VPN was not found.");
                syncResult.IsSuccess = false;
            }
            else
            {
                syncResult = await NetSync.DeleteFromNetworkAsync("/ip-vpn/vpn/" + vpn.Name);
            }

            return syncResult;
        }

        /// <summary>
        /// Validate a VPN before syncing to the network
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        private ServiceResult ValidateVpn(Vpn vpn)
        {
            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;

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

            return validationResult;
        }
    }
}