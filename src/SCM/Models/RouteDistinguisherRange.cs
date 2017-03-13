using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SCM.Models
{
    public class RouteDistinguisherRange
    {
        public int RouteDistinguisherRangeID { get; set; }
        [Required]
        public string Name { get; set; }
        public int AdministratorSubField { get; set; }
        public int AssignedNumberSubFieldStart { get; set; }
        public int AssignedNumberSubFieldCount { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public ICollection<Vrf> Vrfs { get; set; }
    }
}