using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SCM.Models.NetModels.IpVpnNetModels
{

    public class RouteTargetNetModel
    {
        [XmlElement(ElementName = "administrator-subfield")]
        public string AdministratorSubField { get; set; }
        [XmlElement(ElementName = "assigned-number-subfield")]
        public string AssignedNumberSubField { get; set; }
    }

    public class TenantPrefixNetModel
    {
        [XmlElement(ElementName = "prefix")]
        public string Prefix { get; set; }
        [XmlElement(ElementName = "is-bgp-learned")]
        public bool IsBgpLearned { get; set; }
    }

    public class TenantCommunityNetModel
    {
        [XmlElement(ElementName = "autonomous-system-number")]
        public int AutonomousSystemNumber { get; set; }
        [XmlElement(ElementName = "number")]
        public int Number { get; set; }
    }

    public class PENetModel
    {
        [XmlElement(ElementName = "pe-name")]
        public string PEName { get; set; }
        [XmlElement(ElementName = "vrf")]
        public List<VrfNetModel> Vrfs { get; set; }
    }

    public class VrfNetModel
    {
        [XmlElement(ElementName = "vrf-name")]
        public string VrfName { get; set; }
        [XmlElement(ElementName = "preference")]
        public int Preference { get; set; }
    }   

    public class VpnAttachmentSetNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "pe")]
        public List<PENetModel> PEs { get; set; }
        [XmlElement(ElementName = "is-hub")]
        public bool? IsHub { get; set; }
        public bool ShouldSerializeIsHub()
        {
            return IsHub.HasValue;
        }
        [XmlElement(ElementName = "tenant-ipv4-prefix")]
        public List<TenantPrefixNetModel> TenantPrefixes { get; set; }
        [XmlElement(ElementName = "tenant-community")]
        public List<TenantCommunityNetModel> TenantCommunities { get; set; }
    }

    [XmlRoot(ElementName = "vpn", Namespace = "urn:thomsonreuters:ip-vpn")]
    public class IpVpnServiceNetModel
    {
        [XmlElement(ElementName = "name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "topology-type")]
        public string TopologyType { get; set; }
        [XmlElement(ElementName = "route-target-A")]
        public RouteTargetNetModel RouteTargetA { get; set; }
        [XmlElement(ElementName = "route-target-B")]
        public RouteTargetNetModel RouteTargetB { get; set; }
        [XmlElement(ElementName = "is-extranet")]
        public bool? IsExtranet { get; set; }
        public bool ShouldSerializeIsExtranet()
        {
            return IsExtranet.HasValue;
        }
        [XmlElement(ElementName = "vpn-attachment-set")]
        public List<VpnAttachmentSetNetModel> VpnAttachmentSets { get; set; }
    }
}
