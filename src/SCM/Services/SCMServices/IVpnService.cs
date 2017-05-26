using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SCM.Models;
using SCM.Data;

namespace SCM.Services.SCMServices
{
    public interface IVpnService
    {
        IUnitOfWork UnitOfWork { get; }

        Task<IEnumerable<Vpn>> GetAllAsync();
        Task<IEnumerable<Vpn>> GetAllByVrfIDAsync(int id);
        Task<IEnumerable<Vpn>> GetAllByAttachmentSetIDAsync(int id);
        Task<IEnumerable<Vpn>> GetAllByTenantNetworkIDAsync(int id);
        Task<IEnumerable<Vpn>> GetAllByTenantCommunityIDAsync(int id);
        Task<Vpn> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(Vpn vpn);
        Task<int> UpdateAsync(Vpn vpn);
        Task<ServiceResult> DeleteAsync(Vpn vpn);
        Task<ServiceResult> ValidateNewAsync(Vpn vpn);
        Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn);
        ServiceResult ShallowCheckNetworkSync(IEnumerable<Vpn> vpns);
        Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn);
        Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn, AttachmentSet attachmentSetContext);
        Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Vpn> vpns, IProgress<ServiceResult> progress);
        Task<IEnumerable<ServiceResult>> CheckNetworkSyncAsync(IEnumerable<Vpn> vpns, AttachmentSet attachmentSetContext, IProgress<ServiceResult> progress);
        Task<ServiceResult> SyncToNetworkAsync(Vpn vpn);
        Task<ServiceResult> SyncToNetworkAsync(Vpn vpn, AttachmentSet attachmentSetContext);
        Task<IEnumerable<ServiceResult>> SyncToNetworkAsync(IEnumerable<Vpn> vpns, AttachmentSet attachmentSetContext, IProgress<ServiceResult> progress);
        Task<ServiceResult> DeleteFromNetworkAsync(Vpn vpn);
        Task UpdateRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges);
        Task UpdateRequiresSyncAsync(Vpn vpn, bool requiresSync, bool saveChanges);
        Task UpdateRequiresSyncAsync(IEnumerable<Vpn> vpns, bool requiresSync, bool saveChanges);
    }
}
