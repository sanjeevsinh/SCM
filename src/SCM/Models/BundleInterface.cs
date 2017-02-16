using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class BundleInterface
    {
        public int BundleInterfaceID { get; set; }
        [Required]
        [MaxLength(15)]
        public string Name { get; set; }
        public int InterfaceBandwidthID { get; set; }
        public bool IsTagged { get; set; }
        public bool IsLayer3 { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        public int DeviceID { get; set; }
        public int? VrfID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
        public virtual ICollection<BundleInterfacePort> BundleInterfacePorts { get; set; }
        public virtual ICollection<BundleInterfaceVlan> BundleInterfaceVlans { get; set; }
    }
}