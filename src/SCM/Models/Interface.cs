using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace SCM.Models
{
    public class Interface { 

        [ForeignKey("Port")]
        public int ID { get; set; }
        public bool IsTagged { get; set; }
        public bool IsLayer3 { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int InterfaceBandwidthID { get; set; }
        public int? VrfID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
        public virtual Port Port { get; set; }
        public virtual ContractBandwidthPool ContractBandwidthPool { get; set; }
        public virtual ICollection<InterfaceVlan> InterfaceVlans { get; set; }
    }
}