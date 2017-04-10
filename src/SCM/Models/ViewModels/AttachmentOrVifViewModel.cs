using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentOrVifViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Attachment Name")]
        public string AttachmentName { get; set; }
        [Display(Name = "Bundle")]
        public bool AttachmentIsBundle { get; set; }
        [Display(Name = "Multi-Port")]
        public bool AttachmentIsMultiPort { get; set; }
        [Display(Name = "VIF")]
        public bool IsVif { get; set; }
        [Display(Name = "VIF Name")]
        public string VifName { get; set; }
        [Display(Name = "Attachment Set")]
        public string AttachmentSetName { get; set; }
        [Display(Name = "Tenant")]
        public string TenantName { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        [Display(Name = "VRF")]
        public string VrfName { get; set; }
        [Display(Name = "Device")]
        public string DeviceName { get; set; }
        public string Region { get; set; }
        [Display(Name = "Sub-Region")]
        public string SubRegion { get; set; }
        public string LocationName { get; set; }
        [Display(Name = "Interface Bandwidth (Gbps)")]
        public int InterfaceBandwidthValue { get; set; }
        [Display(Name = "Plane")]
        public string PlaneName { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public string ContractBandwidthPoolName { get; set; }
        [Display(Name = "Contract Bandwidth (Mbps)")]
        public int ContractBandwidthValue { get; set; }
        [Display(Name = "Requires Sync")]
        public bool RequiresSync { get; set; }
    }
}
