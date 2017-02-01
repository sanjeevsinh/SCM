using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class VpnTenancyTypeViewModel
    {
        [Display(AutoGenerateField = false)]
        public int VpnTenancyTypeID { get; set; }
        [Required]
        [Display(Name ="Tenancy Type")]
        [StringLength(50)]
        public string TenancyType { get; set; }
        public byte[] RowVersion { get; set; }
    }
}