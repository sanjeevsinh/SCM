using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class VifRequestViewModel : IValidatableObject
    {
        public int AttachmentID { get; set; }
        [Display(Name = "Vlan Tag")]
        [Range(2, 4094, ErrorMessage = "The vlan tag must be a number between 2 and 4094.")]
        public int VlanTag { get; set; }
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
        public int? ContractBandwidthPoolID { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public ContractBandwidthPoolViewModel ContractBandwidthPool { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsLayer3)
            {
                if (string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address must be specified for layer 3 vifs.");
                }
                if (string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask must be specified for layer 3 vifs.");
                }
                if (string.IsNullOrEmpty(VrfName))
                {
                    yield return new ValidationResult(
                        "A VRF name must be specified for layer 3 vifs.");
                }
                if (VrfAdministratorSubField == null)
                {
                    yield return new ValidationResult(
                        "A VRF Administrator Sub-Field must be specified for layer 3 vifs");
                }
                if (VrfAssignedNumberSubField == null)
                {
                    yield return new ValidationResult(
                        "A VRF Assigned Number Sub-Field must be specified for layer 3 vifs.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address can only be specified for layer 3 vifs.");
                }

                if (!string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask can only be specified for layer 3 vifs.");
                }

                if (!string.IsNullOrEmpty(VrfName))
                {
                    yield return new ValidationResult(
                        "A VRF name can only be specified for layer 3 vifs.");
                }

                if (VrfAdministratorSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Administrator Sub-Field can only be specified for layer 3 vifs.");
                }
                if (VrfAssignedNumberSubField != null)
                {
                    yield return new ValidationResult(
                        "A VRF Assigned Number Sub-Field can only be specified for layer 3 vifs.");
                }
            }
        }
    }
}
