﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace SCM.Models
{
    public class Vrf
    {
        public int VrfID { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        [Required]
        public int AdministratorSubField { get; set; }
        [Required]
        public int AssignedNumberSubField { get; set; }
        public int DeviceID { get; set; }
        public int TenantID { get; set; }
        public int RouteDistinguisherRangeID { get; set; }
        public bool IsLayer3 { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Device Device { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ICollection<Attachment> Attachments { get; set; }
        public virtual ICollection<Vif> Vifs { get; set; }
        public virtual ICollection<BgpPeer> BgpPeers { get; set; }
        public virtual ICollection<AttachmentSetVrf> AttachmentSetVrfs { get; set; }
        public virtual RouteDistinguisherRange RouteDistinguisherRange { get; set; }

    }
}