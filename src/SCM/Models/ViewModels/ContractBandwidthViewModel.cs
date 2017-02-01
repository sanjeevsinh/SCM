using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class ContractBandwidthViewModel
    {
        [Display(AutoGenerateField = false)]
        public int PolicyBandwidthID { get; set; }
        public int BandwidthKbps { get; set; }
        public byte[] RowVersion { get; set; }
    }
}