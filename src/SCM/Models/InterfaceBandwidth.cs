using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class InterfaceBandwidth
    {
        public int InterfaceBandwidthID { get; set; }
        public int BandwidthGbps { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}