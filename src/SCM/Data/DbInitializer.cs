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
                new Location {SiteName = "FR5", SubRegionID = subregions.Single(s => s.Name == "Frankfurt").SubRegionID, Tier = 1}
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

            var interfaceBandwidths = new List<InterfaceBandwidth>
            {
                new InterfaceBandwidth {BandwidthGbps = 1 },
                new InterfaceBandwidth {BandwidthGbps = 10 },
                new InterfaceBandwidth {BandwidthGbps = 20 },
                new InterfaceBandwidth {BandwidthGbps = 40 },
                new InterfaceBandwidth {BandwidthGbps = 100 }
            };

            foreach (InterfaceBandwidth p in interfaceBandwidths)
            {
                context.InterfaceBandwidth.Add(p);
            }

            var contractBandwidths = new List<ContractBandwidth>
            {
                new ContractBandwidth {BandwidthKbps = 10 },
                new ContractBandwidth {BandwidthKbps = 20 },
                new ContractBandwidth {BandwidthKbps = 30 },
                new ContractBandwidth {BandwidthKbps = 40 },
                new ContractBandwidth {BandwidthKbps = 50 },
                new ContractBandwidth {BandwidthKbps = 100 }
            };

            foreach (ContractBandwidth p in contractBandwidths)
            {
                context.ContractBandwidths.Add(p);
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

            context.SaveChanges();
        }
    }
}