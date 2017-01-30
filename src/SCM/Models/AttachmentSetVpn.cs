using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class AttachmentSetVpn
    {
        public int AttachmentSetVpnID { get; set; }
        public int AttachmentSetID { get; set; }
        public int VpnID { get; set; }
        public AttachmentSet AttachmentSet { get; set; }
        public Vpn Vpn { get; set; }
    }
}