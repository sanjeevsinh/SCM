using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentSetService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<AttachmentSet>> GetAllAsync();
        Task<AttachmentSet> GetByIDAsync(int id);
        Task<int> AddAsync(AttachmentSet attachmentSet);
        Task<int> UpdateAsync(AttachmentSet attachmentSet);
        Task<int> DeleteAsync(AttachmentSet attachmentSet);
        Task<ServiceResult> ValidateNewAsync(AttachmentSet attachmentSet);
        Task<ServiceResult> ValidateChangesAsync(AttachmentSet attachmentSet);
    }
}
