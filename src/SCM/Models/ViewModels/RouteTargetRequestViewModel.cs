using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class RouteTargetRequestViewModel
    {
        [Display(Name = "Administrator Sub-Field")]
        [Required]
        [Range(1, 4294967295)]
        public int AdministratorSubField { get; set; }
        [Display(Name = "Request Assigned Number Sub-Field")]
        [Range(1, 4294967295)]
        public int? RequestedAssignedNumberSubField { get; set; }
        [Display(Name = "Auto-Allocate Assigned Number Sub-Field")]
        public bool AutoAllocateAssignedNumberSubField { get; set; }
        [Display(Name = "Hub Export")]
        public bool IsHubExport { get; set; }
        public int VpnID { get; set; }
    }
}