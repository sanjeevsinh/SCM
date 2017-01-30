using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class VpnProtocolTypeViewModel
    {
        [Required(ErrorMessage = "A VPN Protocol Type must be selected")]
        public int VpnProtocolTypeID { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "Protocol Type")]
        public string ProtocolType { get; set; }
        public byte[] RowVersion { get; set; }

    }
}