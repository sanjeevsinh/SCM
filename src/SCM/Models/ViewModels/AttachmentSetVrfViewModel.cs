using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class AttachmentSetVrfViewModel
    {
        [Display(AutoGenerateField = false)]
        public int AttachmentSetVrfID { get; set; }
        public int AttachmentSetID { get; set; }
        [Required(ErrorMessage = "A VRF must be selected.")]
        public int VrfID { get; set; }
        [Display(Name = "Attachment Set")]
        public AttachmentSetViewModel AttachmentSet { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
        public byte[] RowVersion { get; set; }
    }
}