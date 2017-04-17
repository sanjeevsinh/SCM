using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models.ViewModels
{
    public class AttachmentSetVrfViewModel
    {
        [Display(AutoGenerateField = false)]
        public int AttachmentSetVrfID { get; set; }
        public int AttachmentSetID { get; set; }
        [Required(ErrorMessage = "A VRF must be selected.")]
        public int VrfID { get; set; }
        [Range(1, 500,ErrorMessage = "Enter a number between 1 and 500")]
        public int? Preference { get; set; }
        [Display(Name = "Attachment Set")]
        public AttachmentSetViewModel AttachmentSet { get; set; }
        [Display(Name = "VRF Name")]
        public VrfViewModel Vrf { get; set; }
        [Display(Name = "Attachment/VIF Name")]
        public string AttachmentOrVifName { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public string ContractBandwidthPoolName { get; set; }
        [Display(Name = "Region")]
        public string RegionName { get; set; }
        [Display(Name = "Sub-Region")]
        public string SubRegionName { get; set; }
        [Display(Name = "Location")]
        public string LocationSiteName { get; set; }
        [Display(Name = "Device")]
        public string DeviceName { get; set; }
        [Display(Name = "Plane")]
        public string PlaneName { get; set; }
        public byte[] RowVersion { get; set; }
    }
}