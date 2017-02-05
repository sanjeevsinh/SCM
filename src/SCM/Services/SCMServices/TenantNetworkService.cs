using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class TenantNetworkService : BaseService, ITenantNetworkService
    {
        public TenantNetworkService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<TenantNetwork>> GetAllAsync()
        {
            return await this.UnitOfWork.TenantNetworkRepository.GetAsync();
        }

        public async Task<TenantNetwork> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.TenantNetworkRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(TenantNetwork tenantNetwork)
        {
            this.UnitOfWork.TenantNetworkRepository.Insert(tenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(TenantNetwork tenantNetwork)
        {
            this.UnitOfWork.TenantNetworkRepository.Update(tenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(TenantNetwork tenantNetwork)
        {
            this.UnitOfWork.TenantNetworkRepository.Delete(tenantNetwork);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
