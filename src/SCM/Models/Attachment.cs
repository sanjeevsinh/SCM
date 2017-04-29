using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class Attachment { 

        public int AttachmentID { get; set; }
        [NotMapped]
        public string Name
        {
            get
            {
                if (IsBundle)
                {
                    return $"Bundle{ID}";
                }
                else if (IsMultiPort)
                {
                    return $"MultiPort{ID}";
                }
                else
                {
                    var port = Interfaces.Single().Ports.Single();
                    return $"{port.Type} {port.Name}";
                }
            }
        }
        public bool IsTagged { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsBundle { get; set; }
        public bool IsMultiPort { get; set; }
        public int? ID { get; set; }
        public int AttachmentBandwidthID { get; set; }
        public int TenantID { get; set; }
        public int DeviceID { get; set; }
        public int? VrfID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public bool RequiresSync { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Device Device { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual AttachmentBandwidth AttachmentBandwidth { get; set; }
        public virtual ContractBandwidthPool ContractBandwidthPool { get; set; }
        public virtual ICollection<Interface> Interfaces { get; set; }
        public virtual ICollection<Vif> Vifs { get; set; }
    }
}