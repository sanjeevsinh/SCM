using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public interface INetworkSyncService
    {
        Task<NetworkCheckSyncServiceResult> CheckNetworkSyncAsync(Object item, string resource);
        Task<NetworkSyncServiceResult> SyncNetworkAsync(object item, string resource);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(string resource);
    }
}
