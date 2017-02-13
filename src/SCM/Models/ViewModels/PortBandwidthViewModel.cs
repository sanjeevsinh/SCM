using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models.ViewModels
{
    public class PortBandwidthViewModel
    {
        [Display(AutoGenerateField = false)]
        public int PortBandwidthID { get; set; }
        public int BandwidthGbps { get; set; }
        public byte[] RowVersion { get; set; }
    }
}