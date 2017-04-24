using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class AttachmentRequestViewModel : IValidatableObject
    {
        [Display(Name = "Layer 3 Enabled", Description = "Check this option to request a layer 3 attachment.")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "Bundle Required", Description = "Check this option to request a bundle attachment.")]
        public bool BundleRequired { get; set; }
        [Display(Name = "Multi-Port Required",Description ="Check this option to request a multi-port attachment.")]
        public bool MultiPortRequired { get; set; }
        [Display(Name = "Tagged")]
        public bool IsTagged { get; set; }
        [Required(ErrorMessage = "A tenant must be selected")]
        public int TenantID { get; set; }
        public TenantViewModel Tenant { get; set; }
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
        [Display(Name = "Attachment Bandwidth (Gigabits/Second)")]
        public AttachmentBandwidthViewModel Bandwidth { get; set; }
        public int? ContractBandwidthID { get; set; }
        [Display(Name = "Trust Received COS and DSCP")]
        public bool TrustReceivedCosDscp { get; set; }
        [Display(Name = "Contract Bandwidth (Megabits/Second)")]
        public ContractBandwidthViewModel ContractBandwidth { get; set; }
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IsTagged)
            {
                if (!string.IsNullOrEmpty(IpAddress1) || !string.IsNullOrEmpty(IpAddress2) 
                    || !string.IsNullOrEmpty(IpAddress3) || !string.IsNullOrEmpty(IpAddress4))
                {
                    yield return new ValidationResult(
                        "An IP address cannot be specified for tagged attachments.");
                }

                if (!string.IsNullOrEmpty(SubnetMask1) || !string.IsNullOrEmpty(SubnetMask2) 
                    || !string.IsNullOrEmpty(SubnetMask3) || !string.IsNullOrEmpty(SubnetMask4))
                {
                    yield return new ValidationResult(
                        "A subnet mask cannot be specified for tagged attachments.");
                }
                if (ContractBandwidthID != null)
                {
                    yield return new ValidationResult(
                        "A Contract Bandwidth cannot be specified for tagged attachments.");
                }
            }
            else
            {
                if (ContractBandwidthID == null)
                {
                    yield return new ValidationResult(
                        "A Contract Bandwidth must be specified for untagged attachments.");
                }
            }

            if (!IsLayer3)   
            {
                if (!string.IsNullOrEmpty(IpAddress1) || !string.IsNullOrEmpty(IpAddress2) 
                    || !string.IsNullOrEmpty(IpAddress3) || !string.IsNullOrEmpty(IpAddress4))
                {
                    yield return new ValidationResult(
                        "IP addresses can only be specified for layer 3 attachments.");
                }

                if (!string.IsNullOrEmpty(SubnetMask1) || !string.IsNullOrEmpty(SubnetMask2) 
                    || !string.IsNullOrEmpty(SubnetMask3) || !string.IsNullOrEmpty(SubnetMask4))
                {
                    yield return new ValidationResult(
                        "Subnet masks can only be specified for layer 3 attachments.");
                }
            }
        }
    }
}
