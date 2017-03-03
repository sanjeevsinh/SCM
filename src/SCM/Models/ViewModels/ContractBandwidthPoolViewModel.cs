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
        public string Name { get; set; }
        public int ContractBandwidthID { get; set; }
        [Display(Name = "Trust received COS or DSCP markings")]
        public bool TrustReceivedCosDscp { get; set; }
        [Display(Name = "Contract Bandwidth (Kbps)")]
        public ContractBandwidthViewModel ContractBandwidth { get; set; }
    }
}
