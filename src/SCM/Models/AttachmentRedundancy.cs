using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class AttachmentRedundancy
    {
        public int AttachmentRedundancyID { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}