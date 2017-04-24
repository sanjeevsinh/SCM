using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Models.ServiceModels;
using SCM.Data;
using System.Linq.Expressions;

namespace SCM.Services.SCMServices
{
    public interface IAttachmentService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Attachment> GetByIDAsync(int id);
        Task<Attachment> GetByVrfIDAsync(int vrfID);
        Task<List<Attachment>> GetAllByVpnIDAsync(int vpnID);
        Task<List<Attachment>> GetAllByTenantAsync(Tenant tenant);
        Task<List<Attachment>> GetAsync(Expression<Func<Attachment, bool>> filter = null);
        Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest);
        Task<ServiceResult> DeleteAsync(Attachment attachment);
        Task<ServiceResult> CheckNetworkSyncAsync(Attachment attachment);
        Task<ServiceResult> SyncToNetworkAsync(Attachment attachment);
        Task<ServiceResult> DeleteFromNetworkAsync(Attachment attachment);
        Task<ServiceResult> ValidateNewAsync(AttachmentRequest request);
        Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(Attachment attachment, bool requiresSync, bool saveChanges = true);
    }
}
