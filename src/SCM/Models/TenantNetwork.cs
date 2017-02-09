using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class TenantNetwork
    {
        public int TenantNetworkID { get; set; }
        [MaxLength(15)]
        public string IpPrefix { get; set; }
        [Required]
        [Range(1,32)]
        public int Length { get; set; }
        public bool AllowExtranet { get; set; }
        public int TenantID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}