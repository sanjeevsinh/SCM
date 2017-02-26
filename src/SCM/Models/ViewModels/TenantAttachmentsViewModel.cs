using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;


namespace SCM.Models.ViewModels
{
    public class TenantAttachmentsViewModel
    {
       [Display(Name ="Attachment Ports")]
       public IEnumerable<AttachmentInterfaceViewModel> AttachmentInterfaces { get; set; }
       [Display(Name = "Attachment Bundles")]
       public IEnumerable<AttachmentBundleInterfaceViewModel> AttachmentBundleInterfaces { get; set; }
    }
}
