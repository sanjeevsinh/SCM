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
        public bool IsLayer3 { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int InterfaceID { get; set; }
        [Range(2,4094)]
        public int VlanTag { get; set; }
        public int VrfID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Interface Interface { get; set; }
        public virtual Vrf Vrf { get; set; }
    }
}