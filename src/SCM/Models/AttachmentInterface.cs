﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models
{
    public class AttachmentInterface
    {
        public int ID { get; set; }
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
        public int? VrfID { get; set; }
        public Port Port { get; set; }
        public Tenant Tenant { get; set; }
        public Device Device { get; set; }
        public Region Region { get; set; }
        public SubRegion SubRegion { get; set; }
        public Location Location { get; set; }
        public InterfaceBandwidth Bandwidth { get; set; }
        public Plane Plane { get; set; }
        public Vrf Vrf { get; set; }
    }
}
