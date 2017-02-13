using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public interface INetworkSyncService
    {
        Task<NetworkSyncServiceResult> SyncToNetwork(object item, string resource);
    }
}
