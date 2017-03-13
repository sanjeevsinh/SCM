using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class RouteTargetRange
    {
        public int RouteTargetRangeID { get; set; }
        [Required]
        public string Name { get; set; }
        public int AdministratorSubField { get; set; }
        public int AssignedNumberSubFieldStart { get; set; }
        public int AssignedNumberSubFieldCount { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<RouteTarget> RouteTargets  { get; set; }
    }
}