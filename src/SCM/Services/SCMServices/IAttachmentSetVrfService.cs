using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;
using SCM.Models.ServiceModels;

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
        Task<ServiceResult> ValidateAsync(AttachmentSetVrf attachmentSetVrf);
        Task<ServiceResult> ValidateDeleteAsync(AttachmentSetVrf attachmentSetVrf);
        Task<IEnumerable<Vrf>> GetCandidateVrfs(AttachmentSetVrfRequest request);
        Task<ServiceResult> CheckVrfsConfiguredCorrectlyAsync(AttachmentSet attachmentSet);
    }
}
