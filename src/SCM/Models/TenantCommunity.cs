using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class TenantCommunity
    {
        public int TenantCommunityID { get; set; }
        [Required]
        public int AutonomousSystemNumber { get; set; }
        [Required]
        public int Number { get; set; }
        public bool AllowExtranet { get; set; }
        [Required]
        public int TenantID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}