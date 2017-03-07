using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models
{
    public class Tenant
    {
        public int TenantID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Timestamp] 
        public byte[] RowVersion { get; set; }
        public virtual ICollection<TenantNetwork> TenantNetworks { get; set; }
        public virtual ICollection<TenantCommunity> TenantCommunities { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual ICollection<Interface> Interfaces { get; set; }
    }
}