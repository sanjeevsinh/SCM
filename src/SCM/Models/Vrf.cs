using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models
{
    public class Vrf
    {
        public int VrfID { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        public int AdministratorSubField { get; set; }
        [Required]
        public int AssignedNumberSubField { get; set; }
        public int DeviceID { get; set; }
        public int TenantID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Tenant Tenant { get; set; }
        public ICollection<BgpPeer> BgpPeers { get; set; }
    }
}