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
        public VpnTenantNetworkService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<VpnTenantNetwork>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnTenantNetworkRepository.GetAsync(includeProperties: "TenantNetwork,VpnAttachmentSet");
        }

        public async Task<VpnTenantNetwork> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnTenantNetworkRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Insert(vpnTenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Update(vpnTenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnTenantNetwork vpnTenantNetwork)
        {
            this.UnitOfWork.VpnTenantNetworkRepository.Delete(vpnTenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }

        /// <summary>
        /// Validates a Tenant Network for binding to a VPN Attachment Set.
        /// </summary>
        /// <param name="vpnTenantNetwork"></param>
        /// <returns></returns>
        public async Task<ServiceResult> ValidateVpnTenantNetworkAsync (VpnTenantNetwork vpnTenantNetwork)
        {

            var validationResult = new ServiceResult();
            validationResult.IsSuccess = true;
                  
            var dbResult = await UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnTenantNetwork.VpnAttachmentSetID,
                includeProperties:"Vpn", AsTrackable:false);
            var vpnAttachmentSet = dbResult.SingleOrDefault();

            if (vpnAttachmentSet == null)
            {
                validationResult.Add("The Attachment Set was not found.");
                validationResult.IsSuccess = false;
            }

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

                var existingVpnTenantNetworkResult = await UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.TenantNetworkID == vpnTenantNetwork.TenantNetworkID,
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