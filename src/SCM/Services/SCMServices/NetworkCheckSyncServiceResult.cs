using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Services.SCMServices
{
    public class NetworkCheckSyncServiceResult
    {
        public NetworkCheckSyncServiceResult()
        {
            NetworkSyncServiceResult = new NetworkSyncServiceResult();
        }

        public bool InSync { get; set; }
        public NetworkSyncServiceResult NetworkSyncServiceResult { get; set; } 
    }
}
