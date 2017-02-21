using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class VpnTenantCommunity
    {
        public int VpnTenantCommunityID { get; set; }
        public int TenantCommunityID { get; set; }
        public int VpnAttachmentSetID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual TenantCommunity TenantCommunity { get; set; }
        public virtual VpnAttachmentSet VpnAttachmentSet { get; set; }
    }
}