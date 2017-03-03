using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models
{
    public class ContractBandwidthPool
    {
        public int ContractBandwidthPoolID { get; set; }
        public string Name { get; set; }
        public int ContractBandwidthID { get; set; }
        public bool TrustReceivedCosDscp { get; set; }
        public virtual ContractBandwidth ContractBandwidth { get; set; }
    }
}
