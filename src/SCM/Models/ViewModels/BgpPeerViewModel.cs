using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class BgpPeerViewModel
    {
        public int BgpPeerID { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address for the peer must be entered, e.g. 192.168.0.1")]
        [Display(Name = "Peer IP Address")]
        [Required(ErrorMessage = "A peer IP address must be defined.")]
        public string IpAddress { get; set; }
        [Required(ErrorMessage = "A peer autonomous system number must be entered between 1 and 65535")]
        [Range(1,65535)]
        [Display(Name = "Peer Autonomous System")]
        public int AutonomousSystem { get; set; }
        [Display(Name = "Maximum Routes")]
        public int? MaximumRoutes { get; set; }
        public int VrfID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
    }
}