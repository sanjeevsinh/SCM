using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SCM.Models.ServiceModels;


namespace SCM.Models.ViewModels
{
    public class MultiPortVifViewModel
    {
        public string MemberAttachmentName { get; set; }
        public int VlanTag { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public string VrfName { get; set; }
    }
}
