﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class InterfaceViewModel
    {
        [Display(AutoGenerateField = false)]
        public int ID { get; set; }
        [Display(Name ="Tagging Enabled")]
        public bool IsTagged { get; set; }
        [Display(Name = "Layer 3 Enabled")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        public int AttachmentBandwidthID { get; set; }
        public byte[] RowVersion { get; set; }
        [Display(Name ="Attachment Bandwidth (Gbps)")]
        public AttachmentBandwidthViewModel AttachmentBandwidth { get; set; }
    }
}
