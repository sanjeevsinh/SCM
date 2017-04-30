using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ViewModels
{

    public class VpnViewModel
    {
        [Display(AutoGenerateField = false)]
        public int VpnID { get; set; }
        [Required]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "The name must contain letters, numbers, and dashes (-) only and no whitespace.")]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        [Display(Name="Extranet")]
        public bool IsExtranet { get; set; }
        [Required(ErrorMessage = "A VPN Topology Type must be selected.")]
        public int? VpnTopologyTypeID { get; set; }
        [Required(ErrorMessage = "A VPN Tenancy Type must be selected.")]
        public int? VpnTenancyTypeID { get; set; }
        [Required(ErrorMessage = "A Tenant must be selected.")]
        public int? TenantID { get; set; }
        public int? PlaneID { get; set; }
        public int? RegionID { get; set; }
        [Display(Name = "Requires Sync")]
        public bool RequiresSync { get; set; }
        public byte[] RowVersion { get; set; }
        public TenantViewModel Tenant { get; set; }
        public PlaneViewModel Plane { get; set; }
        public RegionViewModel Region { get; set; }
        [Display(Name="Topology Type")]
        public VpnTopologyTypeViewModel VpnTopologyType { get; set; }
        [Display(Name = "Tenancy Type")]
        public VpnTenancyTypeViewModel VpnTenancyType { get; set; }
        /// <summary>
        /// This property provides Attachment Set context for the AttachmentSetVpn controller.
        /// It is not used to update the model.
        /// </summary>
        public AttachmentSetViewModel AttachmentSet { get; set; }
        public int? AttachmentSetID { get; set; } 
    }
}