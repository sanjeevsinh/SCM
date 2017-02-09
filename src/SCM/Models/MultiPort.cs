using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class MultiPort
    {
        public int MultiPortID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string BgpPeerSourceIpAddress { get; set; } 
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<MultiPortPort> MultiPortPorts { get; set; }
    }
}