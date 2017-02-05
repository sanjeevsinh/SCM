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
        public int VpnAttachmentSetID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual TenantNetwork TenantNetwork { get; set; }
        public virtual VpnAttachmentSet VpnAttachmentSet { get; set; }
    }
}