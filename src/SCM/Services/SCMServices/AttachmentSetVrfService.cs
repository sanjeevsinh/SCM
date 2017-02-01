using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentSetVrfService : BaseService, IAttachmentSetVrfService
    {
        public AttachmentSetVrfService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<AttachmentSetVrf>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetVrfRepository.GetAsync(includeProperties: "Tenant,ContractBandwidth,SubRegion.Region,AttachmentRedundancy");
        }

        public async Task<AttachmentSetVrf> GetByIDAsync(int key)
        {
            return await UnitOfWork.AttachmentSetVrfRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Insert(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Update(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(AttachmentSetVrf attachmentSetVrf)
        {
            this.UnitOfWork.AttachmentSetVrfRepository.Delete(attachmentSetVrf);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}