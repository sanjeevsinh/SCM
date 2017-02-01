using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class AttachmentRedundancyViewModel
    {
        [Display(AutoGenerateField = false)]
        public int AttachmentRedundancyID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public byte[] RowVersion { get; set; }
    }
}