using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models
{
    public class TenantNetworkBgpPeer
    {
        public int TenantNetworkBgpPeerID { get; set; }
        public int TenantNetworkID { get; set; }
        public int BgpPeerID { get; set; }
        public TenantNetwork TenantNetwork { get; set; }
        public BgpPeer BgpPeer { get; set; }
    }
}
