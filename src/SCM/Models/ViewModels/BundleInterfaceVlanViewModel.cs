using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class BundleInterfaceVlanViewModel : IValidatableObject
    {
        [Display(AutoGenerateField = false)]
        public int BundleInterfaceVlanID { get; set; }
        public int BundleInterfaceID { get; set; }
        [Display(Name = "Layer 3 Enabled")]
        public bool IsLayer3 { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
         ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
         ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask { get; set; }
        [Required(ErrorMessage = "A vlan tag must be entered")]
        [Range(2, 4094, ErrorMessage = "The vlan tag must be between 2 and 4094")]
        [Display(Name = "Vlan Tag")]
        public int VlanTag { get; set; }
        public int? VrfID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
        public BundleInterfaceViewModel BundleInterface { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsLayer3)
            {
                if (string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address must be specified.");
                }

                if (string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask must be specified.");
                }
                if (VrfID == null)
                {
                    yield return new ValidationResult(
                        "A VRF must be selected.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address cannot be specified.");
                }

                if (!string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask cannot be specified.");
                }
            }
        }
    }
}