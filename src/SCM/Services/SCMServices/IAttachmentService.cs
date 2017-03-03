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
        Task<AttachmentInterface> GetByIDAsync(int id);
        Task<AttachmentInterface> GetAttachmentBundleInterfaceByIDAsync(int id);
        Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest);
        Task<ServiceResult> DeleteAsync(AttachmentInterface attachment);
        Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(int attachmentID);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(int attachmentID);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int attachmentID);
    }
}
