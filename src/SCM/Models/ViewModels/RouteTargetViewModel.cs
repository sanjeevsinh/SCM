using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class RouteTargetViewModel
    {
        [Display(AutoGenerateField = false)]
        public int RouteTargetID { get; set; }
        [Display(Name = "Administrator Sub-Field")]
        [Required]
        [Range(1, 4294967295)]
        public string AdministratorSubField { get; set; }
        [Display(Name = "Assigned Number Sub-Field")]
        [Required]
        [Range(1, 4294967295)]
        public string AssignedNumberSubField { get; set; }
        [Display(Name = "Hub Export")]
        public bool IsHubExport { get; set; }
        public int VpnID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "VPN")]
        public VpnViewModel Vpn { get; set; }
    }
}