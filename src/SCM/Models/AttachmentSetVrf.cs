using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class AttachmentSetVrf
    {
        public int AttachmentSetVrfID { get; set; }
        public int AttachmentSetID { get; set; }
        public int VrfID { get; set; }
        public AttachmentSet AttachmentSet { get; set; }
        public Vrf Vrf { get; set; }
    }
}