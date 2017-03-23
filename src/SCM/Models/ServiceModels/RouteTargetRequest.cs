using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models.ServiceModels
{
    public class RouteTargetRequest
    {
        public int AdministratorSubField { get; set; }
        public int? RequestedAssignedNumberSubField { get; set; }
        public bool AutoAllocateAssignedNumberSubField { get; set; }
        public int AllocatedAssignedNumberSubField { get; set; }
        public bool IsHubExport { get; set; }
        public int VpnID { get; set; }
        public int? RouteTargetRangeID { get; set; }
    }
}