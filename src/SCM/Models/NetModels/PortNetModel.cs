using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models.NetModels
{
    public class AsapNetModel
    {
        public int VlanID { get; set; }
        public int LogicalBandwidth { get; set; }
    }

    public class UntaggedAttachmentPortNetModel
    {
        public string PortType { get; set; }
        public string PortID { get; set; }
        public int PhysicalPortBandwidth { get; set; }
    }

    public class TaggedAttachmentPortNetModel : UntaggedAttachmentPortNetModel
    {
        public ICollection<AsapNetModel> Asaps { get; set; }
    }

    public class UntaggedBundlePortNetModel
    {
        public string BundlePortID { get; set; } 
        public ICollection<UntaggedAttachmentPortNetModel> BundlePortMembers { get; set; }
        public int PhysicalPortBandwidth { get; set; }
        public int LogicalBandwidth { get; set; }
    }

    public class TaggedBundlePortNetModel
    {
        public string BundlePortID { get; set; }
        public ICollection<UntaggedAttachmentPortNetModel> BundlePortMembers { get; set; }
        public int PhysicalPortBandwidth { get; set; }
        public ICollection<AsapNetModel> Asaps { get; set; }
    }

    public class PeNetModel
    {
        public string PEName { get; set; }
        public ICollection<UntaggedAttachmentPortNetModel> UntaggedAttachmentPorts { get; set; }
        public ICollection<TaggedAttachmentPortNetModel> TaggedAttachmentPorts { get; set; }
        public ICollection<UntaggedBundlePortNetModel> UntaggedBundlePorts { get; set; }
        public ICollection<TaggedBundlePortNetModel> TaggedBundlePorts { get; set; }
    }

    public class AttachmentPortsServiceNetModel
    {
        public ICollection<PeNetModel> PEs { get; set; }
    }
}