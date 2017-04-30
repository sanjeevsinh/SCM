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

        /// <summary>
        /// Return all Tenant Communities which belong to a Tenant which is associated
        /// with a given VPN Attachment Set.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TenantCommunity>> GetAllByVpnAttachmentSetIDAsync(int id)
        {

            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(id);
            return await UnitOfWork.TenantCommunityRepository.GetAsync(q => q.TenantID == vpnAttachmentSet.AttachmentSet.TenantID);
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
