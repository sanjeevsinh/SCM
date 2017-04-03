using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class MultiPortVlan
    {
        public int MultiPortVlanID { get; set; }
        public bool IsLayer3 { get; set; }
        [Range(2, 4094)]
        public int VlanTag { get; set; }
        public int MultiPortID { get; set; }
        public int? VrfID { get; set; }
        public int TenantID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public int? VlanTagRangeID { get; set; }
        public bool RequiresSync { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual MultiPort MultiPort { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual ContractBandwidthPool ContractBandwidthPool { get; set; }
        public virtual VlanTagRange VlanTagRange { get; set; }
    }
}