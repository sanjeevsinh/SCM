using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentRequestViewModel : IValidatableObject
    {
        [RegularExpression(@"^[a-zA-Z0-9-]+$", ErrorMessage = "The VRF name must contain letters, numbers, and dashes (-) only and no whitespace.")]
        [StringLength(50)]
        [Display(Name = "VRF Name")]
        public string VrfName { get; set; }
        [Display(Name = "VRF Administrator Sub-Field")]
        [Range(1, 4294967295)]
        public int? VrfAdministratorSubField { get; set; }
        [Display(Name = "VRF Assigned Number Sub-Field")]
        [Range(1, 4294967295)]
        public int? VrfAssignedNumberSubField { get; set; }
        [Display(Name = "Layer 3 Enabled", Description = "Check this option to request a layer 3 attachment.")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "Bundle Required", Description = "Check this option to request a bundle attachment.")]
        public bool BundleRequired { get; set; }
        [Display(Name = "Multi-Port Required",Description ="Check this option to request a multi-port attachment.")]
        public bool MultiPortRequired { get; set; }
        [Display(Name = "Tagged")]
        public bool IsTagged { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask { get; set; }
        [Required(ErrorMessage = "A tenant must be selected")]
        public int TenantID { get; set; }
        public TenantViewModel Tenant { get; set; }
        public int DeviceID { get; set; }
        public DeviceViewModel Device { get; set; }
        [Required(ErrorMessage = "A region must be selected")]
        public int RegionID { get; set; }
        public RegionViewModel Region { get; set; }
        [Required(ErrorMessage = "A sub-region must be selected")]
        public int SubRegionID { get; set; }
        [Display(Name = "Sub-Region")]
        public SubRegionViewModel SubRegion { get; set; }
        [Required(ErrorMessage = "A location must be selected")]
        public int LocationID { get; set; }
        public LocationViewModel Location { get; set; }
        public int? PlaneID { get; set; }
        public PlaneViewModel Plane { get; set; }
        [Required(ErrorMessage = "A bandwidth option must be selected")]
        public int BandwidthID { get; set; }
        [Display(Name = "Bandwidth (Gigabits/Second)")]
        public InterfaceBandwidthViewModel Bandwidth { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsTagged)
            {
                if (!string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address cannot be specified for tagged attachments.");
                }

                if (!string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask cannot be specified for tagged attachments.");
                }
                if (!string.IsNullOrEmpty(VrfName))
                {
                    yield return new ValidationResult(
                        "A VRF name cannot be specified for tagged attachments.");
                }
                if (VrfAdministratorSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Administrator Sub-Field cannot be specified for tagged attachments.");
                }
                if (VrfAssignedNumberSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Assigned Number Sub-Field cannot be specified for tagged attachments.");
                }
            }

            if (IsLayer3)
            {
                if (string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address must be specified for layer 3 attachments.");
                }
                if (string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask must be specified for layer 3 attachments.");
                }
                if (string.IsNullOrEmpty(VrfName))
                {
                    yield return new ValidationResult(
                        "A VRF name must be specified for layer 3 attachments.");
                }
                if (VrfAdministratorSubField == null)
                {
                    yield return new ValidationResult(
                        "A VRF Administrator Sub-Field must be specified for layer 3 attachments.");
                }
                if (VrfAssignedNumberSubField == null)
                {
                    yield return new ValidationResult(
                        "A VRF Assigned Number Sub-Field must be specified for layer 3 attachments.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address can only be specified for layer 3 interfaces.");
                }

                if (!string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask can only be specified for layer 3 attachments.");
                }

                if (!string.IsNullOrEmpty(VrfName))
                {
                    yield return new ValidationResult(
                        "A VRF name can only be specified for layer 3 attachments.");
                }

                if (VrfAdministratorSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Administrator Sub-Field can only be specified for layer 3 attachments.");
                }
                if (VrfAssignedNumberSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Assigned Number Sub-Field can only be specified for layer 3 attachments.");
                }
            }
        }
    }
}
