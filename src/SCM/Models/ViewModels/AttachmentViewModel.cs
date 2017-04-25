using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentViewModel
    {
        public int AttachmentID { get; set; }
        public string Name { get; set; }
        public int TenantID { get; set; }
        [Display(Name = "Bundle Attachment")]
        public bool IsBundle { get; set; }
        [Display(Name = "Multi-Port Attachment")]
        public bool IsMultiPort { get; set; }
        public int CountOfMultiPortMembers { get; set; }
        [Display(Name = "Enabled for Layer 3")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "Enabled with Tagging")]
        public bool IsTagged { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        [Display(Name = "Requires Sync")]
        public bool RequiresSync { get; set; }
        public TenantViewModel Tenant { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
        public DeviceViewModel Device { get; set; }
        public RegionViewModel Region { get; set; }
        [Display(Name = "Sub-Region")]
        public SubRegionViewModel SubRegion { get; set; }
        public LocationViewModel Location { get; set; }
        [Display(Name = "Attachment Bandwidth (Gbps)")]
        public AttachmentBandwidthViewModel AttachmentBandwidth { get; set; }
        public PlaneViewModel Plane { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public ContractBandwidthPoolViewModel ContractBandwidthPool { get; set; }

    }
}
