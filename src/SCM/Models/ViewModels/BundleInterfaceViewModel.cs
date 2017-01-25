using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SCM.Models.ViewModels
{
    public class BundleInterfaceViewModel : IValidatableObject
    {
        [Display(AutoGenerateField = false)]
        public int BundleInterfaceID { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
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
        public int DeviceID { get; set; }
        public int? VrfID { get; set; }
        [Required(ErrorMessage = "An interface bandwidth must be selected")]
        public int InterfaceBandwidthID { get; set; }
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        [Display(Name = "VRF")]
        public virtual Vrf Vrf { get; set; }
        [Display(Name = "Interface Bandwidth")]
        public virtual InterfaceBandwidth InterfaceBandwidth { get; set; }
        public ICollection<BundleInterfacePort> BundleInterfacePort { get; set; }
        public ICollection<BundleInterfaceVlan> BundleInterfaceVlans { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsTagged == true)
            {
                if (!string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address cannot be specified for tagged interfaces.");
                }

                if (!string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask cannot be specified for tagged interfaces.");
                }
                if (VrfID != null)
                {
                    yield return new ValidationResult(
                        "A VRF cannot be selected for tagged interfaces.");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(IpAddress))
                {
                    yield return new ValidationResult(
                        "An IP address must be specified for untagged interfaces.");
                }

                if (string.IsNullOrEmpty(SubnetMask))
                {
                    yield return new ValidationResult(
                        "A subnet mask must be specified for untagged interfaces.");
                }
            }
        }
    }
}
