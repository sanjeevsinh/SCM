using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ServiceModels
{
    public class VifRequest
    {
        public int VlanTag { get; set; }
        public string VrfName { get; set; }
        public int? VrfAdministratorSubField { get; set; }
        public int? VrfAssignedNumberSubField { get; set; }
        public bool IsLayer3 { get; set; }
        public string IpAddress { get; set; }
        public string SubnetMask { get; set; }
        public int TenantID { get; set; }
        public int AttachmentID { get; set; }
        public int? ContractBandwidthPoolID { get; set; }
    }
}
