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
        public virtual AttachmentSet AttachmentSet { get; set; }
        public virtual Vrf Vrf { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}