using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class VpnTenantNetworkViewModel
    {
        [Display(AutoGenerateField = false)]
        public int VpnTenantNetworkID { get; set; }
        [Required(ErrorMessage = "A Tenant Network must be selected.")]
        public int TenantNetworkID { get; set; }
        [Required(ErrorMessage = "An Attachment Set must be selected.")]
        public int VpnAttachmentSetID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "Tenant Network")]
        public TenantNetworkViewModel TenantNetwork { get; set; }
        public VpnAttachmentSetViewModel VpnAttachmentSet { get; set; }
    }
}