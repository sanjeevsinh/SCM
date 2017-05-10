using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class ContractBandwidthPoolViewModel
    {
        [Display(AutoGenerateField = false)]
        public int ContractBandwidthPoolID { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "A name must be specified")]
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "The name must contain letters, numbers, and dashes (-) only and no whitespace.")]
        [StringLength(50)]
        [Display(Name = "Contract Bandwidth Pool Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A Contract Bandwidth option must be selected.")]
        public int ContractBandwidthID { get; set; }
        public int TenantID { get; set; }
        [Display(Name = "Trust received COS or DSCP markings")]
        public bool TrustReceivedCosDscp { get; set; }
        [Display(Name = "Contract Bandwidth (Mbps)")]
        public ContractBandwidthViewModel ContractBandwidth { get; set; }
        public TenantViewModel Tenant { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
