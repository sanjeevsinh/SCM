using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<TenantAttachments> GetByTenantAsync(Tenant tenant);
        Task<AttachmentRequest> GetAttachmentInterfaceByIDAsync(int id);
        Task<AttachmentRequest> GetAttachmentBundleInterfaceByIDAsync(int id);
        Task<int> AddAsync(AttachmentRequest attachmentRequest);
        Task<int> DeleteAttachmentInterfaceAsync(AttachmentInterface attachment);
        Task<int> DeleteAttachmentBundleInterfaceAsync(AttachmentBundleInterface attachment);
    }
}
