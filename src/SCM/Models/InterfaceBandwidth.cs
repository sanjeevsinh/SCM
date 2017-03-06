using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class InterfaceBandwidth
    {
        public int InterfaceBandwidthID { get; set; }
        public int BandwidthGbps { get; set; }
        public bool MustBeBundleOrMultiPort { get; set; }
        public bool SupportedByBundle { get; set; }
        public bool SupportedByMultiPort { get; set; }
        public int? BundleOrMultiPortMemberBandwidthGbps { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}