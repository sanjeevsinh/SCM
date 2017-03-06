using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentViewModel
    {
        public int ID { get; set; }
        public int TenantID { get; set; }
        [Display(Name = "Enabled for Layer 3")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "Enabled with Tagging")]
        public bool IsTagged { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        public PortViewModel Port { get; set; }
        [Display(Name = "Multiport Ports")]
        public ICollection<PortViewModel> MultiPortPorts { get; set; }
        [Display(Name = "Bundle Interface Ports")]
        public ICollection<PortViewModel> BundleInterfacePorts { get; set; }
        public TenantViewModel Tenant { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
        public DeviceViewModel Device { get; set; }
        public RegionViewModel Region { get; set; }
        [Display(Name = "Sub-Region")]
        public SubRegionViewModel SubRegion { get; set; }
        public LocationViewModel Location { get; set; }
        [Display(Name = "Interface Bandwidth (Gbps)")]
        public InterfaceBandwidthViewModel Bandwidth { get; set; }
        public PlaneViewModel Plane { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public ContractBandwidthPoolViewModel ContractBandwidthPool { get; set; }
    }
}
