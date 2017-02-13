using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SCM.Models.NetModels
{
    public class Layer3NetModel
    {
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
        [XmlElement(ElementName = "ipv4-address")]
        public string IpAddress { get; set; }
        [XmlElement(ElementName = "ipv4-subnet-mask")]
        public string SubnetMask { get; set; }
        [XmlElement(ElementName = "enable-bgp")]
        public bool EnableBgp { get; set; }
        [XmlElement(ElementName = "bgp-peer")]
        public List<BgpPeerNetModel> BgpPeers { get; set; }
    }

    public class VrfNetModel
    {
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
        [XmlElement(ElementName = "administrator-subfield")]
        public string AdministratorSubField { get; set; }
        [XmlElement(ElementName = "assigned-number-subfield")]
        public string AssignedNumberSubField { get; set; }
    }

    public class BgpPeerNetModel
    {
        [XmlElement(ElementName = "peer-ipv4-address")]
        public string PeerIpv4Address { get; set; }
        [XmlElement(ElementName = "peer-autonomous-system")]
        public int PeerAutonomousSystem { get; set; }
    }

    public class UntaggedAttachmentInterfaceNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceID { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
    }

    public class TaggedAttachmentInterfaceNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceID { get; set; }
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "vif")]
        public List<VifNetModel> Vifs { get; set; } 
    }

    public class BundleInterfaceMemberNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceID { get; set; }
    }

    public class UntaggedAttachmentBundleInterfaceNetModel
    {
        [XmlElement(ElementName = "bundle-interface-id")]
        public string BundleInterfaceID { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [XmlElement(ElementName = "bundle-interface-member")]
        public List<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
    }

    public class TaggedAttachmentBundleInterfaceNetModel
    {
        [XmlElement(ElementName = "bundle-interface-id")]
        public string BundleInterfaceID { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "bundle-interface-member")]
        public List<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
        [XmlElement(ElementName = "vif")]
        public List<VifNetModel> Vifs { get; set; }
    }

    public class VifNetModel
    {
        [XmlElement(ElementName = "vlan-id")]
        public int VlanID { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3Vrf { get; set; }
    }

    public class PeAttachmentNetModel
    {
        [XmlElement(ElementName ="pe-name")]
        public string PEName { get; set; }

        [XmlElement(ElementName = "vrf")]
        public List<VrfNetModel> Vrfs { get; set; }

        [XmlElement(ElementName = "untagged-attachment-interface")]
        public List<UntaggedAttachmentInterfaceNetModel> UntaggedAttachmentInterfaces { get; set; }

        [XmlElement(ElementName = "tagged-attachment-interface")]
        public List<TaggedAttachmentInterfaceNetModel> TaggedAttachmentInterfaces { get; set; }

        [XmlElement(ElementName = "untagged-attachment-bundle-interface")]
        public List<UntaggedAttachmentBundleInterfaceNetModel> UntaggedAttachmentBundleInterfaces { get; set; }

        [XmlElement(ElementName = "tagged-attachment-bundle-interface")]
        public List<TaggedAttachmentBundleInterfaceNetModel> TaggedAttachmentBundleInterfaces { get; set; }
    }

    [XmlRoot(ElementName = "attachment", Namespace = "urn:thomsonreuters:attachment")]
    public class AttachmentServiceNetModel
    {
        [XmlElement(ElementName = "pe")]
        public List<PeAttachmentNetModel> PEs { get; set; }
    }
}