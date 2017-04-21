using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ServiceModels
{
    public class MultiPortVif
    {
        public Attachment MemberAttachment { get; set; }
        public int VlanTag { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int? VrfID { get; set; }
        public Vrf Vrf { get; set; }
        public ContractBandwidthPool ContractBandwidthPool { get; set; }
    }
}
