using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class VpnVrf
    {
        public int VrfID { get; set; }
        public int VpnID { get; set; }
        public Vrf Vrf { get; set; }
        public Vpn Vpn { get; set; }
    }
}