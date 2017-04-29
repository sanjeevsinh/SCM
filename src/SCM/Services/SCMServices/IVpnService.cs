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
        Task<Vpn> GetByIDAsync(int id);
        Task<ServiceResult> AddAsync(Vpn vpn);
        Task<int> UpdateAsync(Vpn vpn);
        Task<ServiceResult> DeleteAsync(Vpn vpn);
        Task<ServiceResult> ValidateNewAsync(Vpn vpn);
        Task<ServiceResult> ValidateChangesAsync(Vpn vpn, Vpn currentVpn);
        Task<ServiceResult> CheckNetworkSyncAsync(Vpn vpn);
        Task<ServiceResult> SyncToNetworkAsync(Vpn vpn);
        Task<ServiceResult> DeleteFromNetworkAsync(Vpn vpn);
        Task UpdateVpnRequiresSyncAsync(int vpnID, bool requiresSync, bool saveChanges);
        Task UpdateVpnRequiresSyncAsync(Vpn vpn, bool requiresSync, bool saveChanges);
        Task UpdateVpnRequiresSyncAsync(IEnumerable<Vpn> vpns, bool requiresSync, bool saveChanges);
    }
}
