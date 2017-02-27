using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentInterfaceViewModel
    {
        public int ID { get; set; }
        [Display(Name = "Enabled for Layer 3")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "Enabled with Tagging")]
        public bool IsTagged { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        public int? VrfID { get; set; }
        public int TenantID { get; set; }
        public int DeviceID { get; set; }
        public int LocationID { get; set; }
        public int RegionID { get; set; }
        public int SubRegionID { get; set; }
        public int PlaneID { get; set; }
        public int BandwidthID { get; set; }
        public PortViewModel Port { get; set; }
        public TenantViewModel Tenant { get; set; }
        public VrfViewModel Vrf { get; set; }
        public DeviceViewModel Device { get; set; }
        public RegionViewModel Region { get; set; }
        [Display(Name = "Sub-Region")]
        public SubRegionViewModel SubRegion { get; set; }
        public LocationViewModel Location { get; set; }
        [Display(Name = "Bandwidth (Gbps)")]
        public InterfaceBandwidthViewModel Bandwidth { get; set; }
    }
}
