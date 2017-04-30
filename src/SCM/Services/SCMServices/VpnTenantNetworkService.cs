using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnTenantNetworkService : BaseService, IVpnTenantNetworkService
    {
        public VpnTenantNetworkService(IUnitOfWork unitOfWork, IVpnService vpnService) : base(unitOfWork)
        {
            VpnService = vpnService;
        }

        private IVpnService VpnService { get; set; }

        public async Task<IEnumerable<VpnTenantNetwork>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnTenantNetworkRepository.GetAsync(includeProperties: "VpnAttachmentSet.Vpn,"
                + "VpnAttachmentSet.AttachmentSet.Tenant,"
                + "TenantNetwork", 
                AsTrackable: false);
        }

        public async Task<VpnTenantNetwork> GetByIDAsync(int id)
        {
            var dbResult = await UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.VpnTenantNetworkID == id, 
                includeProperties: "VpnAttachmentSet.Vpn,"
                + "VpnAttachmentSet.AttachmentSet.Tenant,"
                + "TenantNetwork", 
                AsTrackable:false);

            return dbResult.SingleOrDefault();
        }

        public async Task<int> AddAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Insert(vpnTenantNetwork);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantNetwork.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Update(vpnTenantNetwork);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantNetwork.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Delete(vpnTenantNetwork);
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(vpnTenantNetwork.VpnAttachmentSetID);
            await VpnService.UpdateVpnRequiresSyncAsync(vpnAttachmentSet.VpnID, true, false);

            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a Tenant Network for binding to a VPN Attachment Set.
        /// </summary>
        /// <param name="vpnTenantNetwork"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateNewAsync (VpnTenantNetwork vpnTenantNetwork, VpnAttachmentSet vpnAttachmentSet)
        {
            var validationResult = new ServiceResult { IsSuccess = true };

            var vpn = vpnAttachmentSet.Vpn;

            if (vpn.IsExtranet)
            {
                var tenantNetwork = await UnitOfWork.TenantNetworkRepository.GetByIDAsync(vpnTenantNetwork.TenantNetworkID);
                if (!tenantNetwork.AllowExtranet)
                {
                    validationResult.Add("Tenant Network " + tenantNetwork.IpPrefix + "/" + tenantNetwork.Length + " is not enabled for Extranet.");
                    validationResult.IsSuccess = false;
                }
            }
            else
            {
                var existingVpnTenantNetworkResult = await UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => 
                    q.TenantNetworkID == vpnTenantNetwork.TenantNetworkID,
                    includeProperties: "TenantNetwork,VpnAttachmentSet.Vpn", AsTrackable: false);

                var existingVpnTenantNetwork = existingVpnTenantNetworkResult.SingleOrDefault();

                if (existingVpnTenantNetwork != null)
                {
                    validationResult.Add("Tenant Network " + existingVpnTenantNetwork.TenantNetwork.IpPrefix
                        + "/" + existingVpnTenantNetwork.TenantNetwork.Length
                        + " is already bound to VPN " + existingVpnTenantNetwork.VpnAttachmentSet.Vpn.Name + ".");
                    validationResult.IsSuccess = false;
                }
            }

            return validationResult;
        }
    }
}