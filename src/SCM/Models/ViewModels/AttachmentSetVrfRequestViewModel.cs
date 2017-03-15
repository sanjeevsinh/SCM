using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class AttachmentSetVrfRequestViewModel
    {
        public int AttachmentSetID { get; set; }
        public Location Location { get; set; }
        [Required(ErrorMessage = "A location must be selected.")]
        public int? LocationID { get; set; }
        public Plane Plane { get; set; }
        public int? PlaneID { get; set; }
        [Required(ErrorMessage = "A tenant must be selected.")]
        public int? TenantID { get; set; }
    }
}
