using System;
using System.Collections.Generic;
using System.Linq;
using SCM.Data;
using SCM.Models;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class AttachmentSetService : BaseService, IAttachmentSetService
    {
        public AttachmentSetService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public async Task<IEnumerable<AttachmentSet>> GetAllAsync()
        {
            return await this.UnitOfWork.AttachmentSetRepository.GetAsync(includeProperties: "Tenant,ContractBandwidth,SubRegion,Region,AttachmentRedundancy");
        }

        public async Task<AttachmentSet> GetByIDAsync(int key)
        {
            return await UnitOfWork.AttachmentSetRepository.GetByIDAsync(key);
        }

        public async Task<int> AddAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Insert(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> UpdateAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Update(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }

        public async Task<int> DeleteAsync(AttachmentSet attachmentSet)
        {
            this.UnitOfWork.AttachmentSetRepository.Delete(attachmentSet);
            return await this.UnitOfWork.SaveAsync();
        }
    }
}