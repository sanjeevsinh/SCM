using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models
{
    public class Port
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Type { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int PortBandwidthID { get; set; }
        public int DeviceID { get; set; }
        public int? TenantID { get; set; }
        public int? MultiPortID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Interface Interface { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual PortBandwidth PortBandwidth { get; set; }
        public virtual MultiPort MultiPort { get; set; }
    }
}