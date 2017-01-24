using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models
{
    public class Region
    {
        public int RegionID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<SubRegion> SubRegions { get; set; }
    }
}