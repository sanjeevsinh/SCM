using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models.ViewModels
{
    public class InterfaceBandwidthViewModel
    {
        [Display(AutoGenerateField = false)]
        public int InterfaceBandwidthID { get; set; }
        public int BandwidthKbps { get; set; }
        public byte[] RowVersion { get; set; }
    }
}