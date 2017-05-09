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
        Task<IEnumerable<Attachment>> GetAllByVpnIDAsync(int vpnID);
        Task<IEnumerable<Attachment>> GetAllByTenantIDAsync(int tenantID);
        Task<IEnumerable<Attachment>> GetAllByTenantAsync(Tenant tenant);
        Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest);
        Task<ServiceResult> DeleteAsync(Attachment attachment);
        ServiceResult ShallowCheckNetworkSync(IEnumerable<Attachment> attachments);
        Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Attachment> attachments, IProgress<ServiceResult> progress);
        Task<ServiceResult> CheckNetworkSyncAsync(Attachment attachment);
        Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Attachment> attachments, IProgress<ServiceResult> progress);
        Task<ServiceResult> SyncToNetworkAsync(Attachment attachment);
        Task<ServiceResult> DeleteFromNetworkAsync(Attachment attachment);
        Task<ServiceResult> ValidateNewAsync(AttachmentRequest request);
        Task<ServiceResult> ValidateAsync(Vpn vpn);
        Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(Attachment attachment, bool requiresSync, bool saveChanges = true);
    }
}
