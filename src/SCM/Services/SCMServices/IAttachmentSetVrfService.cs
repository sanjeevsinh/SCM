using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentSetVrfService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<AttachmentSetVrf>> GetAllAsync();
        Task<AttachmentSetVrf> GetByIDAsync(int id);
        Task<int> AddAsync(AttachmentSetVrf attachmentSetVrf);
        Task<int> UpdateAsync(AttachmentSetVrf attachmentSetVrf);
        Task<int> DeleteAsync(AttachmentSetVrf attachmentSetVrf);
        Task<ServiceValidationResult> ValidateVrfChangesAsync(AttachmentSetVrf attachmentSetVrf);
        Task<ServiceValidationResult> ValidateVrfsAsync(AttachmentSet attachmentSet);
    }
}
