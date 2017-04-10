﻿using System;
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
        Task<AttachmentAndVifs> GetByIDAsync(int id, bool? multiPort = false);
        Task<AttachmentAndVifs> GetByVrfIDAsync(int vrfID);
        Task<List<AttachmentAndVifs>> GetAllByTenantAsync(Tenant tenant);
        Task<List<AttachmentAndVifs>> GetAsync(Expression<Func<Interface, bool>> filter = null);
        Task<List<AttachmentAndVifs>> GetAsync(Expression<Func<MultiPort, bool>> filter = null);
        Task<ServiceResult> AddAsync(AttachmentRequest attachmentRequest);
        Task<ServiceResult> DeleteAsync(AttachmentAndVifs attachment);
        Task<ServiceResult> CheckNetworkSyncAsync(AttachmentAndVifs attachment);
        Task<ServiceResult> SyncToNetworkAsync(AttachmentAndVifs attachment);
        Task<ServiceResult> DeleteFromNetworkAsync(AttachmentAndVifs attachment);
        Task<ServiceResult> ValidateAsync(AttachmentRequest request);
        Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true, bool? isMultiPort = false);
        Task UpdateRequiresSyncAsync(Interface iface, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(MultiPort multiPort, bool requiresSync, bool saveChanges = true);
    }
}
