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
    public interface IVifService
    {
        IUnitOfWork UnitOfWork { get; }
        Task<Vif> GetByIDAsync(int id);
        Task<Vif> GetByVrfIDAsync(int vrfID);
        Task<List<Vif>> GetAllByAttachmentIDAsync(int id);
        Task<List<Vif>> GetAllByVpnIDAsync(int vpnID);
        Task<List<Vif>> GetAsync(Expression<Func<Vif, bool>> filter = null);
        Task<ServiceResult> AddAsync(VifRequest request);
        Task<ServiceResult> DeleteAsync(Vif vif);
        Task<ServiceResult> CheckNetworkSyncAsync(Vif vif);
        Task<ServiceResult> SyncToNetworkAsync(Vif vif);
        Task<ServiceResult> DeleteFromNetworkAsync(Vif vif);
        Task<ServiceResult> ValidateNewAsync(VifRequest request);
        Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(Vif vif, bool requiresSync, bool saveChanges = true);
    }
}