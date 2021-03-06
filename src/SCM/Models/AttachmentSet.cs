﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;

namespace SCM.Models
{
    public class AttachmentSet
    {
        public int AttachmentSetID { get; set; }
        [MaxLength(50)]
        [Required]
        public string Name { get; set; }
        public bool IsLayer3 { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        public int TenantID { get; set; }
        public int AttachmentRedundancyID { get; set; }
        public int RegionID { get; set; }
        public int? SubRegionID { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual AttachmentRedundancy AttachmentRedundancy { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual Region Region { get; set; }
        public virtual SubRegion SubRegion { get; set; }
        public virtual ICollection<AttachmentSetVrf> AttachmentSetVrfs { get; set; }
        public virtual ICollection<VpnAttachmentSet> VpnAttachmentSets { get; set; }
    }
}