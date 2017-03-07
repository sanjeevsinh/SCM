using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models
{
    public class BundleInterfacePort
    {
        public int BundleInterfacePortID { get; set; }
        public int InterfaceID { get; set; }
        public int PortID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Port Port { get; set; }
        public virtual Interface Interface { get; set; }
    }
}