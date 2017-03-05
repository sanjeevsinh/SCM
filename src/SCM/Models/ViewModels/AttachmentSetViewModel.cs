using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class AttachmentSetViewModel
    {
        [Display(AutoGenerateField = false)]
        public int AttachmentSetID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(250)]
        public string Description { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        [Required(ErrorMessage = "An Attachment Redundancy option must be selected.")]
        public int AttachmentRedundancyID { get; set; }
        [Required(ErrorMessage = "A Region option must be selected.")]
        public int RegionID { get; set; }
        public int? SubRegionID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "Attachment Redundancy")]
        public virtual AttachmentRedundancyViewModel AttachmentRedundancy { get; set; }
        [Display(Name = "Tenant")]
        public virtual TenantViewModel Tenant { get; set; }
        [Display(Name = "Region")]
        public virtual RegionViewModel Region { get; set; }
        [Display(Name = "Sub-Region")]
        public virtual SubRegionViewModel SubRegion { get; set; }
    }
}