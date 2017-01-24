using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class DeviceViewModel
    {
        [Display(AutoGenerateField = false)]
        public int ID { get; set; }
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A device name must be specified")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The device name must contain letters and numbers only and no whitespace.")]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public byte[] RowVersion { get; set; }
        [Required(ErrorMessage = "A plane must be selected")]
        public int PlaneID { get; set; }
        public PlaneViewModel Plane { get; set; }
        [Required(ErrorMessage = "A location must be selected")]
        public int LocationID { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
