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
        [RegularExpression(@"^([1-9]|[1-8][0-9]|9[0-9]|[1-8][0-9]{2}|9[0-8][0-9]|99[0-9]|[1-8][0-9]{3}|9[0-8][0-9]{2}|99[0-8][0-9]|999[0-9]|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]):([1-9]|[1-8][0-9]|9[0-9]|[1-8][0-9]{2}|9[0-8][0-9]|99[0-9]|[1-8][0-9]{3}|9[0-8][0-9]{2}|99[0-8][0-9]|999[0-9]|[1-8][0-9]{4}|9[0-8][0-9]{3}|99[0-8][0-9]{2}|999[0-8][0-9]|9999[0-9])$",
            ErrorMessage = "The administrator subfield must be in the format (1 - 65535):(1 - 99999), e.g. 8718:100")]
        public string AdministratorSubField { get; set; }
        [Display(Name = "Assigned Number Sub-Field")]
        [Required]
        [RegularExpression(@"^([1-9]|[1-8][0-9]|9[0-9]|[1-8][0-9]{2}|9[0-8][0-9]|99[0-9]|[1-8][0-9]{3}|9[0-8][0-9]{2}|99[0-8][0-9]|999[0-9]|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]):([1-9]|[1-8][0-9]|9[0-9]|[1-8][0-9]{2}|9[0-8][0-9]|99[0-9]|[1-8][0-9]{3}|9[0-8][0-9]{2}|99[0-8][0-9]|999[0-9]|[1-8][0-9]{4}|9[0-8][0-9]{3}|99[0-8][0-9]{2}|999[0-8][0-9]|9999[0-9])?$",
           ErrorMessage = "The assigned number subfield must be in the format (1 - 65535):(1 - 99999), e.g. 8718:100")]
        public string AssignedNumberSubField { get; set; }
        [Display(Name = "Hub Export", Prompt = "Tick this box if the route target is for export from a Hub VRF")]
        public bool IsHubExport { get; set; }
        public int VpnID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "VPN")]
        public VpnViewModel Vpn { get; set; }
    }
}