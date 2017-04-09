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
        Task<Vpn> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(Vpn vpn);
        Task<int> UpdateAsync(Vpn vpn);
        Task<ServiceResult> DeleteAsync(Vpn vpn);
        Task<ServiceResult> ValidateAsync(Vpn vpn);
        Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn);
        Task<NetworkSyncServiceResult> CheckNetworkSyncAsync(int vpnID);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(int vpnID);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int vpnID);
        Task UpdateVpnRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges);
        Task UpdateVpnRequiresSyncAsync(Vpn vpn, bool requiresSync, bool saveChanges);
    }
}
