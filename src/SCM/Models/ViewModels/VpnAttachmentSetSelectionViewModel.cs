using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class VpnAttachmentSetSelectionViewModel
    {
        [Required(ErrorMessage = "An VPN must be selected.")]
        public int VpnID { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        public VpnViewModel Vpn { get; set; }
        public TenantViewModel Tenant { get;set;}
    }
}