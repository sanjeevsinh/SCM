using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace SCM.Models
{
    public class Interface { 

        public int InterfaceID { get; set; }
        public int DeviceID { get; set; }
        public bool IsTagged { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsBundle { get; set; }
        public int? BundleID { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int InterfaceBandwidthID { get; set; }
        public int TenantID { get; set; }
        public int? PortID { get; set; }
        public int? VrfID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public bool RequiresSync { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
        public virtual Port Port { get; set; }
        public virtual ContractBandwidthPool ContractBandwidthPool { get; set; }
        public virtual ICollection<InterfaceVlan> InterfaceVlans { get; set; }
        public virtual ICollection<BundleInterfacePort> BundleInterfacePorts { get; set; }
    }
}