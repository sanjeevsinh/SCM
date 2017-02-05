using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface ITenantNetworkService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<TenantNetwork>> GetAllAsync();
        Task<TenantNetwork> GetByIDAsync(int id);
        Task<int> AddAsync(TenantNetwork tenantNetwork);
        Task<int> UpdateAsync(TenantNetwork tenantNetwork);
        Task<int> DeleteAsync(TenantNetwork tenantNetwork);
    }
}
