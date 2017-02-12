using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SCM.Models.NetModels
{
    public class Layer3NetModel
    {
        [JsonProperty(PropertyName = "vrf-name")]
        public string VrfName { get; set; }
        [JsonProperty(PropertyName = "ipv4-address")]
        public string IpAddress { get; set; }
        [JsonProperty(PropertyName = "ipv4-subnet-mask")]
        public string IpSubnetMask { get; set; }
        [JsonProperty(PropertyName = "enable-bgp")]
        public bool EnableBgp { get; set; }
        [JsonProperty(PropertyName = "bgp-peers")]
        public ICollection<BgpPeerNetModel> BgpPeers { get; set; }
    }

    public class VrfNetModel
    {
        [JsonProperty(PropertyName = "vrf-name")]
        public string VrfName { get; set; }
        [JsonProperty(PropertyName = "administrator-subfield")]
        public string AdministratorSubField { get; set; }
        [JsonProperty(PropertyName = "assigned-number-subfield")]
        public string AssignedNumberSubField { get; set; }
    }

    public class BgpPeerNetModel
    {
        [JsonProperty(PropertyName = "peer-ipv4-address")]
        public string PeerIpv4Address { get; set; }
        [JsonProperty(PropertyName = "peer-autonomous-system")]
        public int PeerAutonomousSystem { get; set; }
    }

    public class UntaggedAttachmentInterfaceNetModel
    {
        [JsonProperty(PropertyName = "interface-type")]
        public string InterfaceType { get; set; }
        [JsonProperty(PropertyName = "interface-id")]
        public string InterfaceID { get; set; }
        [JsonProperty(PropertyName = "administrator-subfield")]
        public int InterfaceBandwidth { get; set; }
        [JsonProperty(PropertyName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [JsonProperty(PropertyName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
    }

    public class TaggedAttachmentInterfaceNetModel
    {
        [JsonProperty(PropertyName = "interface-type")]
        public string InterfaceType { get; set; }
        [JsonProperty(PropertyName = "interface-id")]
        public string InterfaceID { get; set; }
        public int InterfaceBandwidth { get; set; }
        [JsonProperty(PropertyName = "vifs")]
        public ICollection<VifNetModel> Vifs { get; set; } 
    }

    public class BundleInterfaceMemberNetModel
    {
        [JsonProperty(PropertyName = "interface-type")]
        public string InterfaceType { get; set; }
        [JsonProperty(PropertyName = "interface-id")]
        public string InterfaceID { get; set; }
    }

    public class UntaggedAttachmentBundleInterfaceNetModel
    {
        [JsonProperty(PropertyName = "bundle-interface-id")]
        public string BundleInterfaceID { get; set; }
        [JsonProperty(PropertyName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [JsonProperty(PropertyName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [JsonProperty(PropertyName = "layer-3")]
        public Layer3NetModel Layer3 { get; set; }
        [JsonProperty(PropertyName = "bundle-interface-members")]
        public ICollection<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
    }

    public class TaggedAttachmentBundleInterfaceNetModel
    {
        [JsonProperty(PropertyName = "bundle-interface-id")]
        public string BundleInterfaceID { get; set; }
        [JsonProperty(PropertyName = "interface-bandwidth")]
        public int InterfaceBandwidth { get; set; }
        [JsonProperty(PropertyName = "bundle-interface-members")]
        public ICollection<BundleInterfaceMemberNetModel> BundleInterfaceMembers { get; set; }
        [JsonProperty(PropertyName = "vifs")]
        public ICollection<VifNetModel> Vifs { get; set; }
    }

    public class VifNetModel
    {
        [JsonProperty(PropertyName = "vlan-id")]
        public int VlanID { get; set; }
        [JsonProperty(PropertyName = "enable-layer-3")]
        public bool EnableLayer3 { get; set; }
        [JsonProperty(PropertyName = "layer-3")]
        public Layer3NetModel Layer3Vrf { get; set; }
    }

    public class PeAttachmentNetModel
    {
        [JsonProperty(PropertyName = "pe-name")]
        public string PEName { get; set; }
        [JsonProperty(PropertyName = "vrfs")]
        public ICollection<VrfNetModel> Vrfs { get; set; }
        [JsonProperty(PropertyName = "untagged-attachment-interfaces")]
        public ICollection<UntaggedAttachmentInterfaceNetModel> UntaggedAttachmentInterfaces { get; set; }
        [JsonProperty(PropertyName = "tagged-attachment-interfaces")]
        public ICollection<TaggedAttachmentInterfaceNetModel> TaggedAttachmentInterfaces { get; set; }
        [JsonProperty(PropertyName = "untagged-attachment-bundle-interfaces")]
        public ICollection<UntaggedAttachmentBundleInterfaceNetModel> UntaggedAttachmentBundleInterfaces { get; set; }
        [JsonProperty(PropertyName = "tagged-attachment-bundle-interfaces")]
        public ICollection<TaggedAttachmentBundleInterfaceNetModel> TaggedAttachmentBundleInterfaces { get; set; }
    }

    public class AttachmentServiceNetModel
    {
        public ICollection<PeAttachmentNetModel> PEs { get; set; }
    }
}