using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace SCM.Models.NetModels.AttachmentNetModels
{
    public class Layer3NetModel
    {
        [XmlElement(ElementName = "ipv4-address")]
        public string IpAddress { get; set; }
        [XmlElement(ElementName = "ipv4-subnet-mask")]
        public string SubnetMask { get; set; }
        [XmlElement(ElementName = "bgp-peer")]
        public List<BgpPeerNetModel> BgpPeers { get; set; }
        public Layer3NetModel()
        {
            BgpPeers = new List<BgpPeerNetModel>();
        }
    }

    public class VrfNetModel
    {
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
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
        [XmlElement(ElementName = "is-bfd-enabled")]
        public bool IsBfdEnabled { get; set; }
    }

    public class ContractBandwidthPoolNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "contract-bandwidth")]
        public int ContractBandwidth { get; set; }
        [XmlElement(ElementName = "trust-received-cos-and-dscp")]
        public bool TrustReceivedCosDscp { get; set; }
    }

    public class PolicyBandwidthNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "bandwidth")]
        public int Bandwidth { get; set; }
    }

    public class TaggedMultiPortPolicyBandwidthNetModel : PolicyBandwidthNetModel
    {
        [XmlElement(ElementName = "contract-bandwidth-pool-name")]
        public string ContractBandwidthPoolName { get; set; }
    }

    public class VifNetModel
    {
        [XmlElement(ElementName = "vlan-id")]
        public int VlanID { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
    }

    public class AttachmentVifNetModel : VifNetModel
    {
        [XmlElement(ElementName = "contract-bandwidth-pool-name")]
        public string ContractBandwidthPoolName { get; set; }
    }

    public class MultiPortVifNetModel : VifNetModel
    {
        [XmlElement(ElementName = "policy-bandwidth-name")]
        public string PolicyBandwidthName { get; set; }
    }

    public class UntaggedAttachmentInterfaceNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceName { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public ContractBandwidthPoolNetModel ContractBandwdithPool { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "policy-bandwidth")]
        public PolicyBandwidthNetModel PolicyBandwidth { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
    }

    public class TaggedAttachmentInterfaceNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceName { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public List<ContractBandwidthPoolNetModel> ContractBandwidthPools { get; set; }
        [XmlElement(ElementName = "vif")]
        public List<AttachmentVifNetModel> Vifs { get; set; } 
        public TaggedAttachmentInterfaceNetModel()
        {
            ContractBandwidthPools = new List<ContractBandwidthPoolNetModel>();
            Vifs = new List<AttachmentVifNetModel>();
        }
    }

    public class BundleInterfaceMemberNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceName { get; set; }
    }

    public class UntaggedAttachmentBundleInterfaceNetModel
    {
        [XmlElement(ElementName = "bundle-interface-id")]
        public int BundleID { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public ContractBandwidthPoolNetModel ContractBandwdithPool { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "policy-bandwidth")]
        public PolicyBandwidthNetModel PolicyBandwidth { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
        [XmlElement(ElementName = "bundle-interface-member")]
        public List<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
        public UntaggedAttachmentBundleInterfaceNetModel ()
        {
            BundleInterfaceMembers = new List<BundleInterfaceMemberNetModel>();
        }
    }

    public class TaggedAttachmentBundleInterfaceNetModel
    {
        [XmlElement(ElementName = "bundle-interface-id")]
        public int BundleID { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public List<ContractBandwidthPoolNetModel> ContractBandwidthPools { get; set; }
        [XmlElement(ElementName = "bundle-interface-member")]
        public List<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
        [XmlElement(ElementName = "vif")]
        public List<AttachmentVifNetModel> Vifs { get; set; }
        public TaggedAttachmentBundleInterfaceNetModel()
        {
            BundleInterfaceMembers = new List<BundleInterfaceMemberNetModel>();
            Vifs = new List<AttachmentVifNetModel>();
            ContractBandwidthPools = new List<ContractBandwidthPoolNetModel>();
        }
    }

    public class UntaggedMultiPortMemberNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceName { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "policy-bandwidth")]
        public PolicyBandwidthNetModel PolicyBandwidth { get; set; }
        [XmlElement(ElementName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
    }

    public class UntaggedAttachmentMultiPortNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public ContractBandwidthPoolNetModel ContractBandwdithPool { get; set; }
        [XmlElement(ElementName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [XmlElement(ElementName = "multiport-member")]
        public List<UntaggedMultiPortMemberNetModel> MultiPortMembers { get; set; }
        public UntaggedAttachmentMultiPortNetModel()
        {
            MultiPortMembers = new List<UntaggedMultiPortMemberNetModel>();
        }
    }

    public class TaggedMultiPortMemberNetModel
    {
        [XmlElement(ElementName = "interface-type")]
        public string InterfaceType { get; set; }
        [XmlElement(ElementName = "interface-id")]
        public string InterfaceName { get; set; }
        [XmlElement(ElementName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [XmlElement(ElementName = "policy-bandwidth")]
        public List<TaggedMultiPortPolicyBandwidthNetModel> PolicyBandwidths { get; set; }
        [XmlElement(ElementName = "vif")]
        public List<MultiPortVifNetModel> Vifs { get; set; }
        public TaggedMultiPortMemberNetModel()
        {
            Vifs = new List<MultiPortVifNetModel>();
            PolicyBandwidths = new List<TaggedMultiPortPolicyBandwidthNetModel>();
        }
    }

    public class TaggedAttachmentMultiPortNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "contract-bandwidth-pool")]
        public List<ContractBandwidthPoolNetModel> ContractBandwidthPools { get; set; }
        [XmlElement(ElementName = "multiport-member")]
        public List<TaggedMultiPortMemberNetModel> MultiPortMembers { get; set; }
        public TaggedAttachmentMultiPortNetModel()
        {
            ContractBandwidthPools = new List<ContractBandwidthPoolNetModel>();
            MultiPortMembers = new List<TaggedMultiPortMemberNetModel>();
        }
    }

    [XmlRoot(ElementName = "pe", Namespace = "urn:thomsonreuters:attachment")]
    public class AttachmentServiceNetModel
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

        [XmlElement(ElementName = "untagged-attachment-multiport")]
        public List<UntaggedAttachmentMultiPortNetModel> UntaggedAttachmentMultiPorts { get; set; }

        [XmlElement(ElementName = "tagged-attachment-multiport")]
        public List<TaggedAttachmentMultiPortNetModel> TaggedAttachmentMultiPorts { get; set; }

        public AttachmentServiceNetModel ()
        {
            Vrfs = new List<VrfNetModel>();
            UntaggedAttachmentInterfaces = new List<UntaggedAttachmentInterfaceNetModel>();
            TaggedAttachmentInterfaces = new List<TaggedAttachmentInterfaceNetModel>();
            UntaggedAttachmentBundleInterfaces = new List<UntaggedAttachmentBundleInterfaceNetModel>();
            TaggedAttachmentBundleInterfaces = new List<TaggedAttachmentBundleInterfaceNetModel>();
            UntaggedAttachmentMultiPorts = new List<UntaggedAttachmentMultiPortNetModel>();
            TaggedAttachmentMultiPorts = new List<TaggedAttachmentMultiPortNetModel>();
        }
    }

    [XmlRoot(ElementName = "vrf", Namespace = "urn:thomsonreuters:attachment")]
    public class VrfServiceNetModel : VrfNetModel
    { 
    }

    [XmlRoot(ElementName = "untagged-attachment-interface", Namespace = "urn:thomsonreuters:attachment")]
    public class UntaggedAttachmentInterfaceServiceNetModel : UntaggedAttachmentInterfaceNetModel
    {
    }

    [XmlRoot(ElementName = "tagged-attachment-interface", Namespace = "urn:thomsonreuters:attachment")]
    public class TaggedAttachmentInterfaceServiceNetModel : TaggedAttachmentInterfaceNetModel
    {
    }

    [XmlRoot(ElementName = "untagged-attachment-bundle-interface", Namespace = "urn:thomsonreuters:attachment")]
    public class UntaggedAttachmentBundleInterfaceServiceNetModel : UntaggedAttachmentBundleInterfaceNetModel
    {
    }

    [XmlRoot(ElementName = "tagged-attachment-bundle-interface", Namespace = "urn:thomsonreuters:attachment")]
    public class TaggedAttachmentBundleInterfaceServiceNetModel : TaggedAttachmentBundleInterfaceNetModel
    {
    }

    [XmlRoot(ElementName = "untagged-attachment-multiport", Namespace = "urn:thomsonreuters:attachment")]
    public class UntaggedAttachmentMultiPortServiceNetModel : UntaggedAttachmentMultiPortNetModel
    {
    }

    [XmlRoot(ElementName = "tagged-attachment-multiport", Namespace = "urn:thomsonreuters:attachment")]
    public class TaggedAttachmentMultiPortServiceNetModel : TaggedAttachmentMultiPortNetModel
    {
    }

    [XmlRoot(ElementName = "vif", Namespace = "urn:thomsonreuters:attachment")]
    public class AttachmentVifServiceNetModel : AttachmentVifNetModel
    {
    }

    [XmlRoot(ElementName = "vif", Namespace = "urn:thomsonreuters:attachment")]
    public class MultiPortVifServiceNetModel : MultiPortVifNetModel
    {
    }
}