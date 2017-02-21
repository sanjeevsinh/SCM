using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using SCM.Models.NetModels.IpVpn;
using System.Threading.Tasks;
using AutoMapper;

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

        public async Task<int> DeleteAsync(Vpn vpn)
        {
            this.UnitOfWork.VpnRepository.Delete(vpn);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validate a new VPN
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        public async Task<ServiceValidationResult> ValidateCreateVpnAsync(Vpn vpn)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            if (vpn.IsExtranet)
            {
                var vpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);

                if (vpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("Extranet is supported only for Hub-and-Spoke IP VPN topologies.");
                    validationResult.IsValid = false;
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
        public async Task<ServiceValidationResult> ValidateVpnChangesAsync(Vpn vpn, Vpn currentVpn)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            if (currentVpn == null)
            {
                validationResult.Add("Unable to save changes. The VPN was deleted by another user.");
                validationResult.IsValid = false;

                return validationResult;
            }

            var attachmentSets = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnID == vpn.VpnID);

            if (attachmentSets.Count() > 0)
            {
                if (vpn.PlaneID != currentVpn.PlaneID)
                {
                    validationResult.Add("The Plane cannot be changed because Attachment Sets are bound to this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.RegionID != currentVpn.RegionID)
                {
                    validationResult.Add("The Region cannot be changed because Attachment Sets are bound to this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.TenantID != currentVpn.TenantID)
                {
                    validationResult.Add("The Tenant Owner cannot be changed because Attachment Sets are bound to this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.VpnTopologyTypeID != currentVpn.VpnTopologyTypeID)
                {
                    validationResult.Add("The Topology Type cannot be changed because Attachment Sets are bound to this VPN.");
                    validationResult.IsValid = false;
                }
                if (vpn.VpnTenancyTypeID != currentVpn.VpnTenancyTypeID)
                {
                    validationResult.Add("The Tenancy Type cannot be changed because Attachment Sets are bound to this VPN.");
                    validationResult.IsValid = false;
                }
            }

            if (vpn.IsExtranet)
            {
                var VpnTopologyType = await UnitOfWork.VpnTopologyTypeRepository.GetByIDAsync(vpn.VpnTopologyTypeID);
                if (VpnTopologyType.TopologyType != "Hub-and-Spoke")
                {
                    validationResult.Add("The Extranet attribute can only be set for VPNs with the 'Hub-and-Spoke' topology.");
                    validationResult.IsValid = false;
                }
            }

            return validationResult;
        }

        public async Task<NetworkSyncServiceResult> CheckSync(int vpnID)
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
                if (!validationResult.IsValid)
                {
                    syncResult.Add(validationResult.GetMessage());
                    syncResult.IsSuccess = false;

                    return syncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                syncResult = await NetSync.CheckSync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

            return syncResult;
        }

        public async Task<NetworkSyncServiceResult> Sync(int vpnID)
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
                if (!validationResult.IsValid)
                {
                    syncResult.Add(validationResult.GetMessage());
                    syncResult.IsSuccess = false;

                    return syncResult;
                }

                var vpnServiceModelData = Mapper.Map<IpVpnServiceNetModel>(vpn);
                syncResult = await NetSync.Sync(vpnServiceModelData, "/ip-vpn/vpn/" + vpn.Name);
            }

            return syncResult;
        }

        /// <summary>
        /// Validate a VPN before syncing to the network
        /// </summary>
        /// <param name="vpn"></param>
        /// <returns></returns>
        private ServiceValidationResult ValidateVpn(Vpn vpn)
        {
            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;

            if (vpn.VpnTopologyType.TopologyType == "Any-to-Any")
            {
                if (vpn.RouteTargets.Count() != 1)
                {
                    validationResult.IsValid = false;
                    validationResult.Add("Route targets must be correctly defined for the VPN.");
                }
            }
            else
            {
                if (vpn.RouteTargets.Count() != 2)
                {
                    validationResult.IsValid = false;
                    validationResult.Add("Route targets must be correctly defined for the VPN.");
                }
            }

            return validationResult;
        }
    }
}