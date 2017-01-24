﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class BundleInterface
    {
        public int BundleInterfaceID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int InterfaceBandwidthID { get; set; }
        public bool IsTagged { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Vrf Vrf { get; set; }
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
        public ICollection<BundleInterfacePort> BundleInterfacePort { get; set; }
        public ICollection<BundleInterfaceVlan> BundleInterfaceVlans { get; set; }
    }
}