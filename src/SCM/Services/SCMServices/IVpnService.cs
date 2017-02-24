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
        Task<int> AddAsync(Vpn vpn);
        Task<int> UpdateAsync(Vpn vpn);
        Task<ServiceResult> DeleteAsync(Vpn vpn);
        Task<ServiceResult> ValidateCreateVpnAsync(Vpn vpn);
        Task<ServiceResult> ValidateVpnChangesAsync(Vpn vpn, Vpn currentVpn);
        Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(int vpnID);
        Task<NetworkSyncServiceResult> SyncToNetworkAsync(int vpnID);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(int vpnID);
    }
}
