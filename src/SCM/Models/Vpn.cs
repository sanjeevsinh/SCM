using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{

    public class Vpn
    {
        public int VpnID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public bool IsExtranet { get; set; }
        public int VpnTopologyTypeID { get; set; }
        public int VpnTenancyTypeID { get; set; }
        public int TenantID { get; set; }
        public int? PlaneID { get; set; }
        public int? RegionID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Plane Plane { get; set; }
        public virtual Region Region { get; set; }
        public virtual VpnTopologyType VpnTopologyType { get; set; }
        public virtual VpnTenancyType VpnTenancyType { get; set; }
        public virtual ICollection<RouteTarget> RouteTargets { get; set; }
        public virtual ICollection<VpnAttachmentSet> VpnAttachmentSets { get; set; }
    }
}