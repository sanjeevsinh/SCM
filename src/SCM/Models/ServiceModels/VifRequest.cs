using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class VifRequest
    {
        public bool AutoAllocateVlanTag { get; set; }
        public int? RequestedVlanTag { get; set; }
        public int? AllocatedVlanTag { get; set; }
        public int? VlanTagRangeID { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress1 { get; set; }
        public string SubnetMask1 { get; set; }
        public string IpAddress2 { get; set; }
        public string SubnetMask2 { get; set; }
        public string IpAddress3 { get; set; }
        public string SubnetMask3 { get; set; }
        public string IpAddress4 { get; set; }
        public string SubnetMask4 { get; set; }
        public int TenantID { get; set; }
        public int AttachmentID { get; set; }
        public int DeviceID { get; set; }
        public bool AttachmentIsMultiPort { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public int? ContractBandwidthID { get; set; }
        public bool TrustReceivedCosDscp { get; set; }
    }
}
