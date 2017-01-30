using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class VpnTopologyTypeViewModel
    {
        public int VpnTopologyTypeID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name ="Topology Type")]
        public string TopologyType { get; set; }
        public int VpnProtocolTypeID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name = "Protocol Type")]
        public VpnProtocolTypeViewModel VpnProtocolType { get; set; }
    }
}