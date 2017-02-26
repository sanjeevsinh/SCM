using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentBundleInterfaceViewModel
    {
        public int ID { get; set; }
        public string InterfaceName { get; set; }
        public string VrfName { get; set; }
        public int VrfAdministratorSubField { get; set; }
        public int VrfAssignedNumberSubField { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsTagged { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int DeviceID { get; set; }
        public int LocationID { get; set; }
        public int RegionID { get; set; }
        public int SubRegionID { get; set; }
        public int PlaneID { get; set; }
        public int BandwidthID { get; set; }
        public TenantViewModel Tenant { get; set; }
        public DeviceViewModel Device { get; set; }
        public RegionViewModel Region { get; set; }
        public SubRegionViewModel SubRegion { get; set; }
        public LocationViewModel Location { get; set; }
        public InterfaceBandwidthViewModel Bandwidth { get; set; }
    }
}
