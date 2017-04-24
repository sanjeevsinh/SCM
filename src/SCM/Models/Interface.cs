using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace SCM.Models
{
    public class Interface { 

        public int InterfaceID { get; set; }
        public int DeviceID { get; set; }
        public int AttachmentID { get; set; }
        [MaxLength(15)]
        public string IpAddress { get; set; }
        [MaxLength(15)]
        public string SubnetMask { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Attachment Attachment { get; set; }
        public virtual Device Device { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual ICollection<Vlan> Vlans { get; set; }
    }
}