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
        public int VlanTagRangeID { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int AttachmentID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
    }
}
