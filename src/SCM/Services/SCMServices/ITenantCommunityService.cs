using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface ITenantCommunityService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<TenantCommunity>> GetAllAsync();
        Task<TenantCommunity> GetByIDAsync(int id);
        Task<int> AddAsync(TenantCommunity tenantCommunity);
        Task<int> UpdateAsync(TenantCommunity tenantCommunity);
        Task<int> DeleteAsync(TenantCommunity tenantCommunity);
    }
}
