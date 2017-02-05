using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class TenantNetworkViewModel
    {
        [Display(AutoGenerateField = false)]
        public int TenantNetworkID { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
           ErrorMessage = "A valid IP prefix must be entered, e.g. 192.168.1.0")]
        [Display(Name = "IP Prefix")]
        public string IpPrefix { get; set; }
        [Required]
        [Range(1,32, ErrorMessage ="A prefix length between 1 and 32 must be entered.")]
        [Display(Name = "Prefix Length")]
        public int Length { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        public byte[] RowVersion { get; set; }
        public Tenant Tenant { get; set; }
    }
}