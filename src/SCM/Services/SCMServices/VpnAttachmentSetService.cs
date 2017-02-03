using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class VpnAttachmentSetService : BaseService, IVpnAttachmentSetService
    {
        public VpnAttachmentSetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<VpnAttachmentSet>> GetAllAsync()
        {
            return await this.UnitOfWork.VpnAttachmentSetRepository.GetAsync();
        }

        public async Task<VpnAttachmentSet> GetByIDAsync(int key)
        {
            return await UnitOfWork.VpnAttachmentSetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Insert(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Update(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(VpnAttachmentSet attachmentSetVpn)
        {
            this.UnitOfWork.VpnAttachmentSetRepository.Delete(attachmentSetVpn);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}