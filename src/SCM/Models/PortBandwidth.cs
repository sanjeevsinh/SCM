using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models
{
    public class PortBandwidth
    {       
        public int PortBandwidthID { get; set; }
        public int BandwidthGbps { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}