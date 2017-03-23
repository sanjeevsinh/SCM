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
        public bool IsBundle { get; set; }
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
        public string Location { get; set; }
        public int InterfaceBandwidth { get; set; }
        public string PlaneName { get; set; }
        public string ContractBandwidthPool { get; set; }
        public int ContractBandwidth { get; set; }
        public bool RequiresSync { get; set; }
    }
}
