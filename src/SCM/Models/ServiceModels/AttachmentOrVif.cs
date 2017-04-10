using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class AttachmentOrVif
    {
        public int ID { get; set; }
        public string AttachmentName { get; set; }
        public bool AttachmentIsBundle { get; set; }
        public bool AttachmentIsMultiPort { get; set; }
        public bool IsVif { get; set; }
        public string VifName { get; set; }
        public string AttachmentSetName { get; set; }
        public string TenantName { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string VrfName { get; set; }
        public string DeviceName { get; set; }
        public string Region { get; set; }
        public string SubRegion { get; set; }
        public string LocationName { get; set; }
        public int InterfaceBandwidthValue { get; set; }
        public string PlaneName { get; set; }
        public string ContractBandwidthPoolName { get; set; }
        public int ContractBandwidthValue { get; set; }
        public bool RequiresSync { get; set; }
    }
}
