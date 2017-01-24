using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class TenantService : BaseService, ITenantService
    {
        public TenantService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await this.UnitOfWork.TenantRepository.GetAsync();
        }

        public async Task<Tenant> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.TenantRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(Tenant tenant)
        {
            this.UnitOfWork.TenantRepository.Insert(tenant);
            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(Tenant tenant)
        {
            this.UnitOfWork.TenantRepository.Update(tenant);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(Tenant tenant)
        {
            this.UnitOfWork.TenantRepository.Delete(tenant);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
