using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class VlanViewModel
    {
        public int VlanID { get; set; }
        [Display(Name = "Layer 3 Enabled")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        public int InterfaceID { get; set; }
        [Display(Name = "Vlan Tag")]
        public int VlanTag { get; set; }
        public byte[] RowVersion { get; set; }
        public InterfaceViewModel Interface { get; set; }
    }
}