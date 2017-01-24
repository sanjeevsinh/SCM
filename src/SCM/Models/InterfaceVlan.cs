using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class InterfaceVlan
    {
        public int InterfaceVlanID { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int InterfaceID { get; set; }
        public int VlanID { get; set; }
        public int InterfaceBandwidthID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Interface Interface { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual Vlan Vlan { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
    }
}