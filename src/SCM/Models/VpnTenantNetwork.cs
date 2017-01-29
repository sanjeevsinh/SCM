using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class VpnTenantNetwork
    {
        public int VpnTenantNetworkID { get; set; }
        public int TenantNetworkID { get; set; }
        public int VpnID { get; set; }
        public TenantNetwork TenantNetwork { get; set; }
        public Vpn Vpn { get; set; }
    }
}