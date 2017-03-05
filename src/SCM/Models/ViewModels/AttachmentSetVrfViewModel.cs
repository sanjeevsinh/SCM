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
        [Range(1, 500,ErrorMessage = "Enter a number between 1 and 500")]
        public int? Preference { get; set; }
        [Display(Name = "Attachment Set")]
        public AttachmentSetViewModel AttachmentSet { get; set; }
        public AttachmentViewModel Attachment { get; set; }
        public byte[] RowVersion { get; set; }
    }
}