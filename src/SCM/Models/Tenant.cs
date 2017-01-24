using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models
{
    public class Tenant
    {
        public int TenantID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Timestamp] 
        public byte[] RowVersion { get; set; }
    }
}