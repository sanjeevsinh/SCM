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
        public bool AttachmentIsMultiPort { get; set; }
        [Display(Name = "Auto-Allocate Vlan Tag")]
        public bool AutoAllocateVlanTag { get; set; }
        [Display(Name = "Request Vlan Tag")]
        [Range(2, 4094, ErrorMessage = "The vlan tag must be a number between 2 and 4094.")]
        public int? RequestedVlanTag { get; set; }      
        [Display(Name = "Layer 3 Enabled", Description = "Check this option to request a layer 3 attachment.")]
        public bool IsLayer3 { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address 1")]
        public string IpAddress1 { get; set; }
        [Display(Name = "Subnet Mask 1")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask1 { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address 2")]
        public string IpAddress2 { get; set; }
        [Display(Name = "Subnet Mask 2")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask2 { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address 3")]
        public string IpAddress3 { get; set; }
        [Display(Name = "Subnet Mask 3")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask3 { get; set; }
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid IP address must be entered, e.g. 192.168.0.1")]
        [Display(Name = "IP Address 4")]
        public string IpAddress4 { get; set; }
        [Display(Name = "Subnet Mask 4")]
        [RegularExpression(@"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$",
            ErrorMessage = "A valid subnet mask must be entered, e.g. 255.255.255.252")]
        public string SubnetMask4 { get; set; }
        [Required(ErrorMessage = "A tenant must be selected")]
        public int TenantID { get; set; }
        public TenantViewModel Tenant { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public ContractBandwidthPoolViewModel ContractBandwidthPool { get; set; }
        public int? ContractBandwidthID { get; set; }
        [Display(Name = "Trust Received COS and DSCP")]
        public bool TrustReceivedCosDscp { get; set; }
        [Display(Name = "Contract Bandwidth (Megabits/Second)")]
        public ContractBandwidthViewModel ContractBandwidth { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsLayer3)
            {
                if (string.IsNullOrEmpty(IpAddress1))
                {
                    yield return new ValidationResult(
                        "An IP address must be specified for layer 3 vifs.");
                }
                if (string.IsNullOrEmpty(SubnetMask1))
                {
                    yield return new ValidationResult(
                        "A subnet mask must be specified for layer 3 vifs.");
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(IpAddress1))
                {
                    yield return new ValidationResult(
                        "An IP address can only be specified for layer 3 vifs.");
                }

                if (!string.IsNullOrEmpty(SubnetMask1))
                {
                    yield return new ValidationResult(
                        "A subnet mask can only be specified for layer 3 vifs.");
                }
            }
            if (!AutoAllocateVlanTag)
            {
                if (RequestedVlanTag == null)
                {
                    yield return new ValidationResult(
                        "A requested vlan tag must be specified, or select the auto-allocate vlan tag option.");
                }
            }
            if (ContractBandwidthID == null && ContractBandwidthPoolID == null)
            {
                yield return new ValidationResult(
                    "Either an existing Contract Bandwidth Pool or a Contract Bandwidth Value must be selected.");
            }
        }
    }
}
