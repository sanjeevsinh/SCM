using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class TenantViewModel
    {
        [Display(AutoGenerateField = false)]
        public int TenantID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "A tenant name must be specified")]
        [RegularExpression(@"^[a-zA-Z0-9]+$",ErrorMessage = "The tenant name must contain letters and numbers only and no whitespace.")]
        [StringLength(30)]
        public string Name { get; set; }
        public byte[] RowVersion { get; set; } 
    }
}
