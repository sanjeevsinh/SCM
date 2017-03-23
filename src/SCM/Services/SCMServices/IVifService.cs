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
        Task<List<Vif>> GetAllByAttachmentIDAsync(int id);
        Task<List<Vif>> GetAsync(Expression<Func<InterfaceVlan, bool>> filter = null);
        Task<ServiceResult> AddAsync(VifRequest request);
        Task<ServiceResult> DeleteAsync(Vif vif);
        Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Vif vif);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(Vif vif);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(Vif vif);
        Task<ServiceResult> ValidateAsync(VifRequest request);
        Task UpdateRequiresSyncAsync(int interfaceVlanID, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(InterfaceVlan ifaceVlan, bool requiresSync, bool saveChanges = true);
    }
}