using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Attachment> GetByIDAsync(int id);
        Task<List<Attachment>> GetAllByTenantAsync(Tenant tenant);
        Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest);
        Task<ServiceResult> DeleteAsync(Attachment attachment);
        Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Attachment attachment);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(Attachment attachment);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(Attachment attachment);
        Task<ServiceResult> ValidateAsync(AttachmentRequest request);
    }
}
