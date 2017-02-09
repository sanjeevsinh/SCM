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
            return await this.UnitOfWork.VpnTenantNetworkRepository.GetAsync(includeProperties: "VpnAttachmentSet,VpnTenantNetwork");
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

        public async Task<ServiceValidationResult> ValidateVpnTenantNetworkAsync (VpnTenantNetwork vpnTenantNetwork)
        {

            var validationResult = new ServiceValidationResult();
            validationResult.IsValid = true;
                  
            var dbRsesult = UnitOfWork.VpnAttachmentSetRepository.GetAsync(q => q.VpnAttachmentSetID == vpnTenantNetwork.VpnAttachmentSetID,
                includeProperties:"Vpn", AsTrackable:false);
            var vpn = await UnitOfWork.VpnRepository.GetByIDAsync(vpnTenantNetwork.VpnAttachmentSet.VpnID);

            if (vpn.IsExtranet)
            {
                var tenantNetwork = await UnitOfWork.TenantNetworkRepository.GetByIDAsync(vpnTenantNetwork.TenantNetworkID);
                if (!tenantNetwork.AllowExtranet)
                {
                    validationResult.Add("Tenant Network " + tenantNetwork.IpPrefix + "/" + tenantNetwork.Length + " is not enabled for Extranet.");
                    validationResult.IsValid = false;
                }
            }

            var existingVpnTenantNetworkResult = await UnitOfWork.VpnTenantNetworkRepository.GetAsync(q => q.TenantNetworkID == vpnTenantNetwork.TenantNetworkID, 
                includeProperties: "TenantNetwork,VpnAttachmentSet.Vpn", AsTrackable: false);
            var existingVpnTenantNetwork = existingVpnTenantNetworkResult.SingleOrDefault();

            if (existingVpnTenantNetwork != null)
            {
                validationResult.Add("Tenant Network " + existingVpnTenantNetwork.TenantNetwork.IpPrefix 
                    + "/" + existingVpnTenantNetwork.TenantNetwork.Length 
                    + " is already bound to VPN " + vpnTenantNetwork.VpnAttachmentSet.Vpn.Name + ".");
                validationResult.IsValid = false;
            }

            return validationResult;
        }
    }
}