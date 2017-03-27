using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class AttachmentRequest
    {
        public bool BundleRequired { get; set; }
        public bool MultiPortRequired { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsTagged { get; set; }
        public string IpAddress1 { get; set; }
        public string SubnetMask1 { get; set; }
        public string IpAddress2 { get; set; }
        public string SubnetMask2 { get; set; }
        public string IpAddress3 { get; set; }
        public string SubnetMask3 { get; set; }
        public string IpAddress4 { get; set; }
        public string SubnetMask4 { get; set; }
        public int TenantID { get; set; }
        public int LocationID { get; set; }
        public int RegionID { get; set; }
        public int SubRegionID { get; set; }
        public int? PlaneID { get; set; }
        public int BandwidthID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public int PortBandwidthRequired { get; set; }
        public int NumPortsRequired { get; set; }
        public InterfaceBandwidth Bandwidth { get; set; }
    }
}
