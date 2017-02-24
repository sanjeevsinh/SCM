using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IVpnTenantNetworkService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<VpnTenantNetwork>> GetAllAsync();
        Task<VpnTenantNetwork> GetByIDAsync(int id);
        Task<int> AddAsync(VpnTenantNetwork vpnTenantNetwork);
        Task<int> UpdateAsync(VpnTenantNetwork vpnTenantNetwork);
        Task<int> DeleteAsync(VpnTenantNetwork vpnTenantNetwork);
        Task<ServiceResult> ValidateVpnTenantNetworkAsync(VpnTenantNetwork vpnTenantNetwork);
    }
}
