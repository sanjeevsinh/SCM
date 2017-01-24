using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class MultiPortPort
    {
        public int MultiPortPortID { get; set; }
        public int PortID { get; set; }
        public int MultiPortID { get; set; }
        public Port Port { get; set; }
        public MultiPort MultiPort { get; set; }
    }
}