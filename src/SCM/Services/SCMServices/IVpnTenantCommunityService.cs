using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IVpnTenantCommunityService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<VpnTenantCommunity>> GetAllAsync();
        Task<VpnTenantCommunity> GetByIDAsync(int id);
        Task<int> AddAsync(VpnTenantCommunity vpnTenantCommunity);
        Task<int> UpdateAsync(VpnTenantCommunity vpnTenantCommunity);
        Task<int> DeleteAsync(VpnTenantCommunity vpnTenantCommunity);
        Task<ServiceResult> ValidateVpnTenantCommunityAsync(VpnTenantCommunity vpnTenantCommunity);
    }
}
