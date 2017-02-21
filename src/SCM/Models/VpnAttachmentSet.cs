using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class VpnAttachmentSet
    {
        public int VpnAttachmentSetID { get; set; }
        public int AttachmentSetID { get; set; }
        public int VpnID { get; set; }
        public virtual AttachmentSet AttachmentSet { get; set; }
        public virtual Vpn Vpn { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<VpnTenantNetwork> VpnTenantNetworks { get; set; }
        public virtual ICollection<VpnTenantCommunity> VpnTenantCommunities { get; set; }
    }
}