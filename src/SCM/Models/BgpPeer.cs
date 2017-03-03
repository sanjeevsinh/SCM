using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class BgpPeer
    {
        public int BgpPeerID { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [Required]
        [Range(1,65535)]
        public int AutonomousSystem { get; set; }
        public int? MaximumRoutes { get; set; }
        public bool IsBfdEnabled { get; set; }
        public int VrfID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Vrf Vrf { get; set; }
    }
}