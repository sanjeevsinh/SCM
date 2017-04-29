using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class Vif
    {
        public int VifID { get; set; }
        public bool IsLayer3 { get; set; }
        public int AttachmentID { get; set; }
        [NotMapped]
        public string Name
        {
            get
            {
                return $"{Attachment.Name}.{VlanTag}";
            }
        }
        [Range(2,4094)]
        public int VlanTag { get; set; }
        public int? VrfID { get; set; }
        public int TenantID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public int? VlanTagRangeID { get; set; }
        public bool RequiresSync { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Attachment Attachment { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual ContractBandwidthPool ContractBandwidthPool { get; set; }
        public virtual VlanTagRange VlanTagRange { get; set; }
        public ICollection<Vlan> Vlans { get; set; }
    }
}