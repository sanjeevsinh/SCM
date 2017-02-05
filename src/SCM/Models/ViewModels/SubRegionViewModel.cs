using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class SubRegionViewModel
    {
        public int SubRegionID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        public int RegionID { get; set; }
        public byte[] RowVersion { get; set; }
        public RegionViewModel Region { get; set; }
    }
}