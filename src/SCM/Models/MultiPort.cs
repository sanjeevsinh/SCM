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
        public int Identifier { get; set; }
        public int TenantID { get; set; }
        public int DeviceID { get; set; }
        public int InterfaceBandwidthID { get; set; }
        public int? VrfID { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsTagged { get; set; }
        public bool RequiresSync { get; set; }
        [Required]
        [MaxLength(15)]
        public string LocalFailureDetectionIpAddress { get; set; }
        [Required]
        [MaxLength(15)]
        public string RemoteFailureDetectionIpAddress { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth {get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual ICollection<MultiPortVlan> MultiPortVlans { get; set; }
    }
}