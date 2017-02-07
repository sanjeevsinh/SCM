using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models.NetModels
{
    public class Layer3VrfNetModel
    {
        public string VrfName { get; set; }
        public string AdministratorSubField { get; set; }
        public string AssignedNumberField { get; set; }
        public string Ipv4Address { get; set; }
        public string Ipv4SubnetMask { get; set; }
        public bool EnableBgp { get; set; }
        public BgpComponentsNetModel Bgp { get; set; }
    }

    public class BgpComponentsNetModel
    {
        public string PeerIpv4Address { get; set; }
        public int PeerAutonomousSystem { get; set; }
    }

    public class UntaggedAttachmentInterfaceNetModel
    {
        public string InterfaceType { get; set; }
        public string InterfaceID { get; set; }
        public int InterfaceBandwidth { get; set; }
        public bool EnableLayer3 { get; set; }
        public Layer3VrfNetModel Layer3Vrf { get; set; }
    }

    public class TaggedAttachmentInterfaceNetModel
    {
        public string InterfaceType { get; set; }
        public string InterfaceID { get; set; }
        public int InterfaceBandwidth { get; set; }
        public ICollection<VifNetModel> Vifs { get; set; } 
    }

    public class UntaggedAttachmentBundleInterfaceNetModel
    {
        public string BundleInterfaceID { get; set; } 
        public int InterfaceBandwidth { get; set; }
        public Layer3VrfNetModel Layer3Vrf { get; set; }
        public ICollection<UntaggedAttachmentInterfaceNetModel> BundleInterfaceMembers { get; set; }
    }

    public class TaggedAttachmentBundleInterfaceNetModel
    {
        public string BundleInterfaceID { get; set; }
        public int InterfaceBandwidth { get; set; }
        public ICollection<UntaggedAttachmentInterfaceNetModel> BundleInterfaceMembers { get; set; }
        public ICollection<VifNetModel> Vifs { get; set; }
    }

    public class VifNetModel
    {
        public int VlanID { get; set; }
        public bool EnableLayer3 { get; set; }
        public Layer3VrfNetModel Layer3Vrf { get; set; }
    }

    public class PeNetModel
    {
        public string PEName { get; set; }
        public ICollection<UntaggedAttachmentInterfaceNetModel> UntaggedAttachmentInterface { get; set; }
        public ICollection<TaggedAttachmentInterfaceNetModel> TaggedAttachmentInterfaces { get; set; }
        public ICollection<UntaggedAttachmentBundleInterfaceNetModel> UntaggedAttachmentBundleInterfaces { get; set; }
        public ICollection<TaggedAttachmentBundleInterfaceNetModel> TaggedAttachmentBundleInterfaces { get; set; }
    }

    public class AttachmentServiceNetModel
    {
        public ICollection<PeNetModel> PEs { get; set; }
    }
}