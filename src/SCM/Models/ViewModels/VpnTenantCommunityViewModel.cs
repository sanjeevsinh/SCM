using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class VpnTenantCommunityViewModel
    {
        public int VpnTenantCommunityID { get; set; }
        public int TenantCommunityID { get; set; }
        public int VpnAttachmentSetID { get; set; }
        public byte[] RowVersion { get; set; }
        public TenantCommunityViewModel TenantCommunity { get; set; }
        public VpnAttachmentSetViewModel VpnAttachmentSet { get; set; }
    }
}