using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class AttachmentRequest
    {
        public string VrfName { get; set; }
        public int? VrfAdministratorSubField { get; set; }
        public int? VrfAssignedNumberSubField { get; set; }
        public bool BundleRequired { get; set; }
        public bool MultiPortRequired { get; set; }
        public bool IsLayer3 { get; set; }
        public bool IsTagged { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int LocationID { get; set; }
        public int RegionID { get; set; }
        public int SubRegionID { get; set; }
        public int? PlaneID { get; set; }
        public int BandwidthID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        public InterfaceBandwidth Bandwidth { get; set; }
    }
}
