using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public class TenantCommunityService : BaseService, ITenantCommunityService
    {
        public TenantCommunityService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<TenantCommunity>> GetAllAsync()
        {
            return await this.UnitOfWork.TenantCommunityRepository.GetAsync();
        }

        public async Task<TenantCommunity> GetByIDAsync(int id)
        {
            return await this.UnitOfWork.TenantCommunityRepository.GetByIDAsync(id);
        }

        public async Task<int> AddAsync(TenantCommunity tenantCommunity)
        {
            this.UnitOfWork.TenantCommunityRepository.Insert(tenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }
 
        public async Task<int> UpdateAsync(TenantCommunity tenantCommunity)
        {
            this.UnitOfWork.TenantCommunityRepository.Update(tenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(TenantCommunity tenantCommunity)
        {
            this.UnitOfWork.TenantCommunityRepository.Delete(tenantCommunity);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}
