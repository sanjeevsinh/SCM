using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class AttachmentSetVrfSelectionViewModel
    {
        [Required(ErrorMessage = "An Attachment Set must be selected.")]
        public int AttachmentSetID { get; set; }
        [Required(ErrorMessage = "A Location must be selected.")]
        public int LocationID { get; set; }
        public int? PlaneID { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        public LocationViewModel Location { get; set; }
        public PlaneViewModel Plane { get; set; }
        public TenantViewModel Tenant { get; set; }
    }
}