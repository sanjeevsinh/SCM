using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models
{
    public class ContractBandwidthPool
    {
        public int ContractBandwidthPoolID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int ContractBandwidthID { get; set; }
        public bool TrustReceivedCosDscp { get; set; }
        public int TenantID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ContractBandwidth ContractBandwidth { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Vif> Vifs { get; set; }
    }
}
