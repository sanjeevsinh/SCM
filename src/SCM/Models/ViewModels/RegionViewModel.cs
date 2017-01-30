using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models.ViewModels
{
    public class RegionViewModel
    {
        public int RegionID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}