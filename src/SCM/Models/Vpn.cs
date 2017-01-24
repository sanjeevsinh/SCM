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
        public bool ForceAssistedVpnAttachment { get; set; }
        public int VpnTopologyTypeID { get; set; }
        public int VpnTenancyTypeID { get; set; }
        public int TenantID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Plane Plane { get; set; }
        public virtual Region Region { get; set; }
        public virtual VpnTopologyType VpnTopologyType { get; set; }
        public virtual VpnTenancyType VpnTenancyType { get; set; }
        public ICollection<TenantNetwork> TenantNetworks { get; set; }
        public ICollection<RouteTarget> RouteTargets { get; set; }
    }
}