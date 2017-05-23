using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace SCM.Models
{

    public class Device
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public int LocationID { get; set; }
        public int PlaneID { get; set; }
        public bool Created { get; set; }
        public bool RequiresSync { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Location Location { get; set;}
        public virtual Plane Plane { get; set; }
        public virtual ICollection<Port> Ports { get; set; }
        public virtual ICollection<Interface> Interfaces { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Vrf> Vrfs { get; set; }
    }
}