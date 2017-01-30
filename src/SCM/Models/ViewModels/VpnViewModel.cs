using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{

    public class VpnViewModel
    {
        public int VpnID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        [Display(Name="Extranet")]
        public bool IsExtranet { get; set; }
        [Required(ErrorMessage = "A VPN Topology Type must be selected.")]
        public int VpnTopologyTypeID { get; set; }
        [Required(ErrorMessage = "A VPN Tenancy Type must be selected.")]
        public int VpnTenancyTypeID { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int TenantID { get; set; }
        public int? PlaneID { get; set; }
        public int? RegionID { get; set; }
        public byte[] RowVersion { get; set; }
        public TenantViewModel Tenant { get; set; }
        public PlaneViewModel Plane { get; set; }
        public RegionViewModel Region { get; set; }
        [Display(Name="VPN Topology Type")]
        public VpnTopologyTypeViewModel VpnTopologyType { get; set; }
        [Display(Name = "VPN Tenancy Type")]
        public VpnTenancyTypeViewModel VpnTenancyType { get; set; }
    }
}