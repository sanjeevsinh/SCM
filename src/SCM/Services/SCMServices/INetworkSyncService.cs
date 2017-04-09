using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public interface INetworkSyncService
    {
        Task<NetworkSyncServiceResult> CheckNetworkSyncAsync(Object item, string resource);
        Task<NetworkSyncServiceResult> SyncNetworkAsync(object item, string resource);
        Task<NetworkSyncServiceResult> SyncNetworkAsync(object item, string resource, HttpMethod method);
        Task<NetworkSyncServiceResult> DeleteFromNetworkAsync(string resource);
    }
}
