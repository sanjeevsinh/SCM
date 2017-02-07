using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class RouteTarget
    {
        public int RouteTargetID { get; set; }
        [Required]
        public int AdministratorSubField { get; set; }
        [Required]
        public int AssignedNumberSubField { get; set; }
        public bool IsHubExport { get; set; }
        public int VpnID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Vpn Vpn { get; set; }
    }
}