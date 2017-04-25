using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models.ViewModels
{
    public class BundleInterfacePortViewModel
    {
        [Display(Name ="Device Name")]
        public string DeviceName { get; set; }
        [Display(Name = "Port Type")]
        public string PortType { get; set; }
        [Display(Name = "Port Name")]
        public string PortName { get; set; }
    }
}