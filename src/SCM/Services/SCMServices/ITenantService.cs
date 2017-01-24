using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface ITenantService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant> GetByIDAsync(int id);
        Task<int> AddAsync(Tenant tenant);
        Task<int> UpdateAsync(Tenant tenant);
        Task<int> DeleteAsync(Tenant tenant);
    }
}
