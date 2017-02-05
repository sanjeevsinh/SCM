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
    }
}