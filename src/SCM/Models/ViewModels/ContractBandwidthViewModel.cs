using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class ContractBandwidthViewModel
    {
        [Display(AutoGenerateField = false)]
        public int PolicyBandwidthID { get; set; }
        [Display(Name = "Contract Bandwidth (Mbps))")]
        public int BandwidthMbps { get; set; }
        public byte[] RowVersion { get; set; }
    }
}