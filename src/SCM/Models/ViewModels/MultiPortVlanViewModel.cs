using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SCM.Models.ServiceModels;


namespace SCM.Models.ViewModels
{
    public class MultiPortVlanViewModel
    {
        [Display(Name = "Port Type")]
        public string PortType { get; set; }
        [Display(Name = "Port Name")]
        public string PortName { get; set; }
        [Display(Name = "Vlan Tag")]
        public int VlanTag { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
    }
}
