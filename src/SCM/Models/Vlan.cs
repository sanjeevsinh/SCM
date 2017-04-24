using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class Vlan
    {
        public int VlanID { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int InterfaceID { get; set; }
        public int VifID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Interface Interface { get; set; }
        public virtual Vif Vif { get; set; }
    }
}