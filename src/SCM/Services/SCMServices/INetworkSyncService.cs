using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public interface INetworkSyncService
    {
        Task<NetworkSyncServiceResult> CheckSync(Object item, string resource = "");
        Task<NetworkSyncServiceResult> Sync(object item, string resource);
    }
}
