using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SCM.Models.ViewModels
{
    public class AttachmentBandwidthViewModel
    {
        [Display(AutoGenerateField = false)]
        public int AttachmentBandwidthID { get; set; }
        public int BandwidthGbps { get; set; }
        public byte[] RowVersion { get; set; }
    }
}