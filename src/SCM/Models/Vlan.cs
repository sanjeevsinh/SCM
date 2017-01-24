using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models
{
    public class Vlan
    {
        [Key]
        public int ID { get; set; }
        public int TagID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}