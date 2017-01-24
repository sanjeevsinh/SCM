using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class VpnTopologyType
    {
        public int VpnTopologyTypeID { get; set; }
        [Required]
        [MaxLength(50)]
        public string TopologyType { get; set; }
        public int VpnProtocolTypeID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual VpnProtocolType VpnProtocolType { get; set; }
        public virtual ICollection<Vpn> Vpns { get; set; }
    }
}