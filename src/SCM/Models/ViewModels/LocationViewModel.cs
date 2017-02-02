using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class LocationViewModel
    {
        [Display(AutoGenerateField = false)]
        public int LocationID { get; set; }
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A site name must be specified.")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The site name must contain letters and numbers only and no whitespace.")]
        public string SiteName { get; set; }
        public int? AlternateLocationID { get; set; }
        public LocationViewModel AlternateLocation { get; set; }
        [Display(Name = "Sub-Region")]
        public SubRegionViewModel SubRegion { get; set; }

    }
}
