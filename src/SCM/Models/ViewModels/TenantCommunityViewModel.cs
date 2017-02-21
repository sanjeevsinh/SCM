using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class TenantCommunityViewModel
    {
        [Display(AutoGenerateField = false)]
        public int TenantCommunityID { get; set; }
        [Range(1,65535, ErrorMessage = "An Autonomous System number between 1 and 65535 must be entered.")]
        [Display(Name = "Autonomous System Number")]
        public int AutonomousSystemNumber { get; set; }
        [Required]
        [Range(1,4294967295, ErrorMessage ="A number between 1 and 4294967295 must be entered.")]
        [Display(Name = "Number")]
        public int Number { get; set; }
        [Display(Name = "Allow Extranet")]
        public bool AllowExtranet { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        public byte[] RowVersion { get; set; }
        public Tenant Tenant { get; set; }
    }
}