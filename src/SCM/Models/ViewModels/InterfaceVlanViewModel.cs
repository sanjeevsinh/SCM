using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class InterfaceVlanViewModel
    {
        public int InterfaceVlanID { get; set; }
        [Display(Name = "IP Address")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
                   ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask { get; set; }
        public int InterfaceID { get; set; }
        [Required(ErrorMessage = "An vlan tag must be entered")]
        [Range(2,4094, ErrorMessage = "The vlan tag must be between 2 and 4094")]
        public int VlanTag { get; set; }
        [Required(ErrorMessage = "A VRF must be selected")]
        public int VrfID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Interface Interface { get; set; }
        [Display(Name = "VRF")]
        public virtual Vrf Vrf { get; set; }
    }
}