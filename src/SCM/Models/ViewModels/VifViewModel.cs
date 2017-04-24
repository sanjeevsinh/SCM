using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SCM.Models.ServiceModels;


namespace SCM.Models.ViewModels
{
    public class VifViewModel
    {
        public int VifID { get; set; }
        public int AttachmentID { get; set; }
        public string Name { get; set; }
        [Display(Name = "Vlan Tag")]
        public int VlanTag { get; set; }
        [Display(Name = "Enabled for Layer 3")]
        public bool IsLayer3 { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Subnet Mask")]
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int VrfID { get; set; }
        public int ContractBandwidthPoolID { get; set; }
        [Display(Name = "Requires Sync")]
        public bool RequiresSync { get; set; }
        public TenantViewModel Tenant { get; set; }
        [Display(Name = "Contract Bandwidth Pool")]
        public ContractBandwidthPoolViewModel ContractBandwidthPool { get; set; }
        [Display(Name = "VRF")]
        public VrfViewModel Vrf { get; set; }
    }
}
