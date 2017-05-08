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
        Task<ServiceResult> AddAsync(VifRequest request);
        Task<ServiceResult> DeleteAsync(Vif vif);
        ServiceResult ShallowCheckNetworkSync(IEnumerable<Vif> vifs);
        Task<ServiceResult> CheckNetworkSyncAsync(Vif vif);
        Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Vif> vifs);
        Task<ServiceResult> SyncToNetworkAsync(Vif vif);
        Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Vif> vifs);
        Task<ServiceResult> DeleteFromNetworkAsync(Vif vif);
        Task<ServiceResult> ValidateNewAsync(VifRequest request);
        Task<ServiceResult> ValidateAsync(Vpn vpn);
        Task UpdateRequiresSyncAsync(int id, bool requiresSync, bool saveChanges = true);
        Task UpdateRequiresSyncAsync(Vif vif, bool requiresSync, bool saveChanges = true);
    }
}