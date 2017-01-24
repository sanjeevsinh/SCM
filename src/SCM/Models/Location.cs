using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        [Required]
        [MaxLength(50)]
        public string SiteName { get; set; }
        public int SubRegionID { get; set; }
        [Required]
        public int Tier { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual SubRegion SubRegion { get; set; }
        public virtual Location AlternateLocation { get; set; }
    }
}