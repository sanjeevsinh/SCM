using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class VpnAttachmentSetViewModel
    {
        [Display(AutoGenerateField = false)]
        public int VpnAttachmentSetID { get; set; }
        [Required(ErrorMessage = "An Attachment Set must be selected.")]
        public int AttachmentSetID { get; set; }
        [Required(ErrorMessage = "A VPN must be selected.")]
        public int VpnID { get; set; }
        [Display(Name = "Hub VRF")]
        public bool IsHub { get; set; }
        [Display(Name = "Attachment Set")]
        public AttachmentSetViewModel AttachmentSet { get; set; }
        [Display(Name = "VPN")]
        public VpnViewModel Vpn { get; set; }
        public byte[] RowVersion { get; set; }
    }
}