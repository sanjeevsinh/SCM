using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models.ViewModels
{
    public class VrfViewModel
    {
        [Display(AutoGenerateField = false)]
        public int VrfID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "A name must be specified")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "The name must contain letters and numbers only and no whitespace.")]
        [StringLength(50)]
        public string Name { get; set; }
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
        [Required]
        public int DeviceID { get; set; }
        [Required(ErrorMessage = "A tenant must be selected")]
        public int TenantID { get; set; }
        public byte[] RowVersion { get; set; }
        public Device Device { get; set; }
        public Tenant Tenant { get; set; }
        public ICollection<BgpPeer> BgpPeers { get; set; }
        public ICollection<VpnVrf> VpnVrfs { get; set; }
    }
}