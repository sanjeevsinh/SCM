using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class VpnTenancyType
    {
        public int VpnTenancyTypeID { get; set; }
        [Required]
        [MaxLength(50)]
        public string TenancyType { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual ICollection<Vpn> Vpns { get; set; }
    }
}