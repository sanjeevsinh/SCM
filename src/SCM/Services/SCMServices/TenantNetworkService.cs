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

        /// <summary>
        /// Return all Tenant Networks which belong to a Tenant which is associated
        /// with a given VPN Attachment Set.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TenantNetwork>> GetAllByVpnAttachmentSetIDAsync(int id)
        {
            var vpnAttachmentSet = await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(id);
            return await UnitOfWork.TenantNetworkRepository.GetAsync(q => q.TenantID == vpnAttachmentSet.AttachmentSet.TenantID);
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
