using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SCM.Models;

namespace SCM.Data
{
    public class SigmaContext : DbContext
    {
        public SigmaContext(DbContextOptions<SigmaContext> options) : base(options)
        {
        }

        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<AttachmentSet> AttachmentSet { get; set; }
        public DbSet<AttachmentSetVrf> AttachmentSetVrfs { get; set; }
        public DbSet<VpnAttachmentSet> VpnAttachmentSets { get; set; }
        public DbSet<AttachmentRedundancy> AttachmentRedundancy { get; set; }
        public DbSet<ContractBandwidth> ContractBandwidth { get; set; }
        public DbSet<ContractBandwidthPool> ContractBandwidthPool { get; set; }
        public DbSet<VpnProtocolType> VpnProtocolTypes { get; set; }
        public DbSet<VpnTopologyType> VpnTopologyTypes { get; set; }
        public DbSet<VpnTenancyType> VpnTenancyTypes { get; set; }
        public DbSet<Vpn> Vpns { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<SubRegion> SubRegions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Port> Ports { get; set; }
        public DbSet<Interface> Interfaces { get; set; }
        public DbSet<PortBandwidth> PortBandwidth { get; set; }
        public DbSet<AttachmentBandwidth> AttachmentBandwidth { get; set; }
        public DbSet<Vlan> Vlans { get; set; }
        public DbSet<Vif> Vifs { get; set; }
        public DbSet<Vrf> Vrfs { get; set; }
        public DbSet<BgpPeer> BgpPeers { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantNetwork> TenantNetworks { get; set; }
        public DbSet<VpnTenantNetwork> VpnTenantNetworks { get; set; }
        public DbSet<Plane> Planes { get; set; }
        public DbSet<RouteTargetRange> RouteTargetRanges { get; set; }
        public DbSet<RouteDistinguisherRange> RouteDistinguisherRanges { get; set; }
        public DbSet<VlanTagRange> VlanTagRanges { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Attachment>().ToTable("Attachment");
            builder.Entity<AttachmentSet>().ToTable("AttachmentSet");
            builder.Entity<VpnAttachmentSet>().ToTable("VpnAttachmentSet");
            builder.Entity<AttachmentSetVrf>().ToTable("AttachmentSetVrf");
            builder.Entity<AttachmentRedundancy>().ToTable("AttachmentRedundancy");
            builder.Entity<BgpPeer>().ToTable("BgpPeer");;
            builder.Entity<ContractBandwidth>().ToTable("ContractBandwidth");
            builder.Entity<ContractBandwidthPool>().ToTable("ContractBandwidthPool");
            builder.Entity<Device>().ToTable("Device");
            builder.Entity<Interface>().ToTable("Interface");
            builder.Entity<Vlan>().ToTable("Vlan");
            builder.Entity<Vif>().ToTable("Vif");
            builder.Entity<Location>().ToTable("Location");
            builder.Entity<AttachmentBandwidth>().ToTable("AttachmentBandwidth");
            builder.Entity<PortBandwidth>().ToTable("PortBandwidth");
            builder.Entity<Port>().ToTable("Port");
            builder.Entity<Region>().ToTable("Region");
            builder.Entity<RouteTarget>().ToTable("RouteTarget");
            builder.Entity<SubRegion>().ToTable("SubRegion");
            builder.Entity<Tenant>().ToTable("Tenant");
            builder.Entity<TenantNetwork>().ToTable("TenantNetwork");
            builder.Entity<TenantCommunity>().ToTable("TenantCommunity");
            builder.Entity<Vpn>().ToTable("Vpn");
            builder.Entity<VpnTenantNetwork>().ToTable("VpnTenantNetwork");
            builder.Entity<VpnProtocolType>().ToTable("VpnProtocolType");
            builder.Entity<VpnTenancyType>().ToTable("VpnTenancyType");
            builder.Entity<VpnTopologyType>().ToTable("VpnTopologyType");
            builder.Entity<Plane>().ToTable("Plane");
            builder.Entity<Vrf>().ToTable("Vrf");
            builder.Entity<RouteTargetRange>().ToTable("RouteTargetRange");
            builder.Entity<RouteDistinguisherRange>().ToTable("RouteDistinguisherRange");
            builder.Entity<VlanTagRange>().ToTable("VlanTagRange");

            // Prevent cascade deletes

            builder.Entity<Attachment>()
                    .HasOne(c => c.AttachmentBandwidth)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Attachment>()
                    .HasOne(c => c.Tenant)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.SubRegion)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.Region)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.AttachmentRedundancy)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.Tenant)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ContractBandwidthPool>()
                   .HasOne(c => c.ContractBandwidth)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Device>()
                    .HasOne(c => c.Plane)
                    .WithMany()
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Device>()
                   .HasOne(c => c.Location)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interface>()
                   .HasOne(c => c.Device)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Location>()
                   .HasOne(c => c.AlternateLocation)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Port>()
                   .HasOne(c => c.PortBandwidth)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vrf>()
                  .HasOne(c => c.Tenant)
                  .WithMany()
                  .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vpn>()
                   .HasOne(c => c.Tenant)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vlan>()
                   .HasOne(c => c.Interface)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vpn>()
                   .HasOne(c => c.Plane)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vpn>()
                   .HasOne(c => c.Region)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vpn>()
                   .HasOne(c => c.VpnTopologyType)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vpn>()
                   .HasOne(c => c.VpnTenancyType)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<VpnTopologyType>()
                   .HasOne(c => c.VpnProtocolType)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            // Set Indexes to ensure data uniqueness

            // Names of Attachment Sets must be unique

            builder.Entity<AttachmentSet>()
            .HasIndex(p => new { p.Name }).IsUnique();

            // VPN protocol type options must be unique

            builder.Entity<VpnProtocolType>()
            .HasIndex(p => new { p.ProtocolType }).IsUnique();

            // VPN tenancy and topology options must be unique

            builder.Entity<VpnTenancyType>()
            .HasIndex(p => new { p.TenancyType }).IsUnique();

            builder.Entity<VpnTopologyType>()
            .HasIndex(p => new { p.TopologyType, p.VpnProtocolTypeID }).IsUnique();

            // VPN names must be unique

            builder.Entity<Vpn>()
            .HasIndex(p => new { p.Name }).IsUnique();

            // Route Distinguisher allocations must be unique

            builder.Entity<Vrf>()
            .HasIndex(p => new { p.AdministratorSubField, p.AssignedNumberSubField }).IsUnique();

            // Vrf names must be unique

            builder.Entity<Vrf>()
            .HasIndex(p => new { p.Name }).IsUnique();

            // VIFs which are created under an Attachment must be unique

            builder.Entity<Vif>()
            .HasIndex(p => new { p.AttachmentID, p.VlanTag }).IsUnique();

            // Device names must be unique

            builder.Entity<Device>()
            .HasIndex(p => p.Name).IsUnique();

            // Location names must be unique

            builder.Entity<Location>()
            .HasIndex(p => p.SiteName).IsUnique();

            // Attachment bandwidth options must be unique

            builder.Entity<AttachmentBandwidth>()
            .HasIndex(p => p.BandwidthGbps).IsUnique();

            // Port bandwidth options must be unique

            builder.Entity<PortBandwidth>()
            .HasIndex(p => p.BandwidthGbps).IsUnique();

            // Ports must be unique per Device

            builder.Entity<Port>()
            .HasIndex(p => new { p.Type, p.Name, p.DeviceID }).IsUnique();

            // Region and Sub-Region names must be unique

            builder.Entity<Region>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<SubRegion>()
            .HasIndex(p => p.Name).IsUnique();

            // Route Targets must be unique

            builder.Entity<RouteTarget>()
            .HasIndex(p => new { p.AdministratorSubField, p.AssignedNumberSubField }).IsUnique();

            // Mappings of VRFs to Attachment Sets must be unique

            builder.Entity<AttachmentSetVrf>()
            .HasIndex(p => new { p.AttachmentSetID, p.VrfID }).IsUnique();

            // Mappings of Attachment Sets to VPNs must be unique

            builder.Entity<VpnAttachmentSet>()
            .HasIndex(p => new { p.AttachmentSetID, p.VpnID }).IsUnique();

            // Contract bandwidth options must be unique

            builder.Entity<ContractBandwidth>()
            .HasIndex(p => new { p.BandwidthMbps }).IsUnique();

            // Contract bandwidth pool names must be unique

            builder.Entity<ContractBandwidthPool>()
            .HasIndex(p => new { p.Name }).IsUnique();

            // Attachment Redundancy names must be unique (e.g. Gold)

            builder.Entity<AttachmentRedundancy>()
            .HasIndex(p => new { p.Name }).IsUnique();

            // Tenants must be unique

            builder.Entity<Tenant>()
            .HasIndex(p => p.Name).IsUnique();

            // Prevent duplicate Tenant Networks

            builder.Entity<TenantNetwork>()
            .HasIndex(p => new { p.IpPrefix, p.Length }).IsUnique();

            // Prevent duplicate Tenant Communities

            builder.Entity<TenantCommunity>()
            .HasIndex(p => new { p.AutonomousSystemNumber, p.Number }).IsUnique();

            // Planes must be unique

            builder.Entity<Plane>()
            .HasIndex(p => p.Name).IsUnique();

            // Ensure a Tenant Network is bound to a single VPN Attachment Set by preventing multiple records 
            // for the same Tenant Network

            builder.Entity<VpnTenantNetwork>()
            .HasIndex(p => new { p.TenantNetworkID, p.VpnAttachmentSetID }).IsUnique();

            builder.Entity<BgpPeer>()
            .HasIndex(p => new { p.VrfID, p.IpAddress }).IsUnique();
        }
    }
}