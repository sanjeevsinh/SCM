using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class SubRegion
    {
        public int SubRegionID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public int RegionID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Region Region { get; set; }
    }
}