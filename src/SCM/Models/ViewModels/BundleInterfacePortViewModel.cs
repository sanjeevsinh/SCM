using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models.ViewModels
{
    public class BundleInterfacePortViewModel
    {
        [Display(AutoGenerateField = false)]
        public int BundleInterfacePortID { get; set; }
        public int BundleInterfaceID { get; set; }
        [Required(ErrorMessage = "A port must be selected")]
        public int PortID { get; set; }
        public byte[] RowVersion { get; set; }
        public PortViewModel Port { get; set; }
        public BundleInterfaceViewModel BundleInterface { get; set; }
    }
}