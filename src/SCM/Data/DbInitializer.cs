using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;
using SCM.Models;

namespace SCM.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SigmaContext context)
        {

            context.Database.Migrate();

            if (context.VpnProtocolTypes.ToList().Count > 0)
            {
                return;
            }

            var vpnProtocolTypes = new List<VpnProtocolType>
            {
                new VpnProtocolType {ProtocolType = "IP" },
                new VpnProtocolType {ProtocolType = "Ethernet" }
            };

            foreach (VpnProtocolType v in vpnProtocolTypes)
            {
                context.VpnProtocolTypes.Add(v);
            }

            var vpnTenancyTypes = new[]
            {
                new VpnTenancyType { TenancyType = "Single" },
                new VpnTenancyType { TenancyType = "Multi" }
            };

            foreach (VpnTenancyType v in vpnTenancyTypes)
            {
                context.VpnTenancyTypes.Add(v);
            }
                
            context.SaveChanges();

            var vpnTopologyTypes = new[]
            {
                new VpnTopologyType { TopologyType = "Any-to-Any", VpnProtocolTypeID = vpnProtocolTypes.Single(v => v.ProtocolType == "IP").VpnProtocolTypeID },
                new VpnTopologyType { TopologyType = "Hub-and-Spoke", VpnProtocolTypeID = vpnProtocolTypes.Single(v => v.ProtocolType == "IP").VpnProtocolTypeID },
                new VpnTopologyType { TopologyType = "Point-to-Point", VpnProtocolTypeID = vpnProtocolTypes.Single(v => v.ProtocolType == "Ethernet").VpnProtocolTypeID },
                new VpnTopologyType { TopologyType = "Multipoint", VpnProtocolTypeID = vpnProtocolTypes.Single(v => v.ProtocolType == "Ethernet").VpnProtocolTypeID }
            };

            foreach (VpnTopologyType v in vpnTopologyTypes)
            {
                context.VpnTopologyTypes.Add(v);
            }

            context.SaveChanges();

            var planes = new List<Plane>
            {
                new Plane {Name = "Red" },
                new Plane {Name = "Blue" }
            };

            foreach (Plane p in planes)
            {
                context.Planes.Add(p);
            }

            var regions = new List<Region>
            {
                new Region {Name = "EMEA" },
                new Region {Name = "AMERS" },
                new Region {Name = "ASIAPAC" },
            };

            foreach (Region r in regions)
            {
                context.Regions.Add(r);
            }

            context.SaveChanges();

            var subregions = new List<SubRegion>
            {
                new SubRegion {Name = "UK", RegionID = regions.Single(s => s.Name == "EMEA").RegionID },
                new SubRegion {Name = "Frankfurt", RegionID = regions.Single(s => s.Name == "EMEA").RegionID },
                new SubRegion {Name = "East Coast", RegionID = regions.Single(s => s.Name == "AMERS").RegionID },
                new SubRegion {Name = "Mid West", RegionID = regions.Single(s => s.Name == "AMERS").RegionID },
                new SubRegion {Name = "Hong Kong", RegionID = regions.Single(s => s.Name == "ASIAPAC").RegionID },
                new SubRegion {Name = "Singapore", RegionID = regions.Single(s => s.Name == "ASIAPAC").RegionID },
            };

            foreach (SubRegion s in subregions)
            {
                context.SubRegions.Add(s);
            }

            context.SaveChanges();

            var locations = new List<Location>
            {
                new Location {SiteName = "UK2", SubRegionID = subregions.Single(s => s.Name == "UK").SubRegionID, Tier = 1 },
                new Location {SiteName = "THW", SubRegionID = subregions.Single(s => s.Name == "UK").SubRegionID, Tier = 1 },
                new Location {SiteName = "FR4", SubRegionID = subregions.Single(s => s.Name == "Frankfurt").SubRegionID, Tier = 1},
                new Location {SiteName = "FR5", SubRegionID = subregions.Single(s => s.Name == "Frankfurt").SubRegionID, Tier = 1},
                new Location {SiteName = "NJ2", SubRegionID = subregions.Single(s => s.Name == "East Coast").SubRegionID, Tier = 1},
                new Location {SiteName = "NJH", SubRegionID = subregions.Single(s => s.Name == "East Coast").SubRegionID, Tier = 1}
            };

            foreach (Location l in locations)
            {
                context.Locations.Add(l);
            }

            var portBandwidths = new List<PortBandwidth>
            {
                new PortBandwidth {BandwidthGbps = 1 },
                new PortBandwidth {BandwidthGbps = 10 },
                new PortBandwidth {BandwidthGbps = 40 },
                new PortBandwidth {BandwidthGbps = 100}
            };

            foreach (PortBandwidth p in portBandwidths)
            {
                context.PortBandwidth.Add(p);
            }

            var attachmentBandwidths = new List<AttachmentBandwidth>
            {
                new AttachmentBandwidth {BandwidthGbps = 1 },
                new AttachmentBandwidth {BandwidthGbps = 10 },
                new AttachmentBandwidth
                {
                    BandwidthGbps = 20,
                    BundleOrMultiPortMemberBandwidthGbps = 10,
                    SupportedByBundle = true,
                    SupportedByMultiPort = true,
                    MustBeBundleOrMultiPort = true
                },
                new AttachmentBandwidth {
                    BandwidthGbps = 40,
                    BundleOrMultiPortMemberBandwidthGbps = 10,
                    SupportedByBundle = true,
                    SupportedByMultiPort = true
                },
                new AttachmentBandwidth {BandwidthGbps = 100 }
            };

            foreach (var p in attachmentBandwidths)
            {
                context.AttachmentBandwidth.Add(p);
            }

            var contractBandwidths = new List<ContractBandwidth>
            {
                new ContractBandwidth {BandwidthMbps = 10 },
                new ContractBandwidth {BandwidthMbps = 20 },
                new ContractBandwidth {BandwidthMbps = 30 },
                new ContractBandwidth {BandwidthMbps = 40 },
                new ContractBandwidth {BandwidthMbps = 50 },
                new ContractBandwidth {BandwidthMbps = 100 },
                new ContractBandwidth {BandwidthMbps = 500 },
                new ContractBandwidth {BandwidthMbps = 1000 },
                new ContractBandwidth {BandwidthMbps = 2000 },
                new ContractBandwidth {BandwidthMbps = 4000 },
                new ContractBandwidth {BandwidthMbps = 6000 },
                new ContractBandwidth {BandwidthMbps = 10000 }
            };

            foreach (ContractBandwidth p in contractBandwidths)
            {
                context.ContractBandwidth.Add(p);
            }

            var attachmentRedundancies = new List<AttachmentRedundancy>
            {
                new AttachmentRedundancy {Name = "Bronze" },
                new AttachmentRedundancy {Name = "Silver" },
                new AttachmentRedundancy {Name = "Gold" },
                new AttachmentRedundancy {Name = "Custom" },
            };
            foreach (AttachmentRedundancy p in attachmentRedundancies)
            {
                context.AttachmentRedundancy.Add(p);
            }

            var defaultVlanTagRange = new VlanTagRange()
            {
                Name = "Default",
                VlanTagRangeStart = 2,
                VlanTagRangeCount = 4000
            };
            context.VlanTagRanges.Add(defaultVlanTagRange);

            var defaultRdRange = new RouteDistinguisherRange()
            {
                Name = "Default",
                AdministratorSubField = 8718,
                AssignedNumberSubFieldStart = 1,
                AssignedNumberSubFieldCount = 1000000
            };
            context.RouteDistinguisherRanges.Add(defaultRdRange);

            var defaultRtRange = new RouteTargetRange()
            {
                Name = "Default",
                AdministratorSubField = 8718,
                AssignedNumberSubFieldStart = 1,
                AssignedNumberSubFieldCount = 1000000
            };
            context.RouteTargetRanges.Add(defaultRtRange);

            context.SaveChanges();
        }
    }
}