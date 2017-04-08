using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class Vif
    {
        public int ID { get; set; }
        public int VlanTag { get; set; }
        public int AttachmentID { get; set; }
        public string Name { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int? VrfID { get; set; }
        public int ContractBandwidthPoolID { get; set; }
        public bool RequiresSync { get; set; }
        public Vrf Vrf { get; set; }
        public Tenant Tenant { get; set; }
        public ContractBandwidthPool ContractBandwidthPool { get; set; }
        public Attachment Attachment { get; set; }
        public ICollection<MultiPortVif> MultiPortVifs { get; set; }
    }
    public class MultiPortVif
    {
        public Attachment Attachment { get; set; }
        public int VlanTag { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int? VrfID { get; set; }
        public Vrf Vrf { get; set; }
    }
}
