using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{
    public class PortViewModel
    {
        [Display(AutoGenerateField = false)]
        public int ID { get; set; }
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A port type must be specified, e.g. GigabitEthernet")]
        public string Type { get; set; }
        [StringLength(50)]
        [Required(AllowEmptyStrings = false, ErrorMessage = "A port name must be specified, e.g. 0/0/0/0")]
        public string Name { get; set; }
        public byte[] RowVersion { get; set; }
        [Required(ErrorMessage = "A device must be selected")]
        public int DeviceID { get; set; }
        public DeviceViewModel Device { get; set; }
        public int? TenantID { get; set; }
        public TenantViewModel Tenant { get; set; }
        [Required(ErrorMessage = "A port bandwidth must be selected")]
        public int PortBandwidthID { get; set; }
        [Display(Name="Port Bandwidth (Gbps)")]
        public PortBandwidthViewModel PortBandwidth { get; set; }
    }
}