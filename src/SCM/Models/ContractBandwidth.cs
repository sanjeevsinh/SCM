using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class ContractBandwidth
    {
        public int ContractBandwidthID { get; set; }
        public int BandwidthMbps { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}