using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models
{
    public class TenantAttachments
    {
       public IEnumerable<AttachmentInterface> AttachmentInterfaces { get; set; }
       public IEnumerable<AttachmentBundleInterface> AttachmentBundleInterfaces { get; set; }
    }
}
