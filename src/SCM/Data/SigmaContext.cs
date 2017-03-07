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
        public DbSet<MultiPort> MultiPorts { get; set; }
        public DbSet<MultiPortPort> MultiPortPorts { get; set; }
        public DbSet<Interface> Interfaces { get; set; }
        public DbSet<PortBandwidth> PortBandwidth { get; set; }
        public DbSet<InterfaceBandwidth> InterfaceBandwidth { get; set; }
        public DbSet<BundleInterfacePort> BundleInterfacePorts { get; set; }
        public DbSet<InterfaceVlan> InterfaceVlans { get; set; }
        public DbSet<Vrf> Vrfs { get; set; }
        public DbSet<BgpPeer> BgpPeers { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantNetwork> TenantNetworks { get; set; }
        public DbSet<VpnTenantNetwork> VpnTenantNetworks { get; set; }
        public DbSet<Plane> Planes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<AttachmentSet>().ToTable("AttachmentSet");
            builder.Entity<VpnAttachmentSet>().ToTable("VpnAttachmentSet");
            builder.Entity<AttachmentSetVrf>().ToTable("AttachmentSetVrf");
            builder.Entity<AttachmentRedundancy>().ToTable("AttachmentRedundancy");
            builder.Entity<BgpPeer>().ToTable("BgpPeer");
            builder.Entity<BundleInterfacePort>().ToTable("BundleInterfacePort");
            builder.Entity<ContractBandwidth>().ToTable("ContractBandwidth");
            builder.Entity<ContractBandwidthPool>().ToTable("ContractBandwidthPool");
            builder.Entity<Device>().ToTable("Device");
            builder.Entity<Interface>().ToTable("Interface");
            builder.Entity<InterfaceVlan>().ToTable("InterfaceVlan");
            builder.Entity<Location>().ToTable("Location");
            builder.Entity<InterfaceBandwidth>().ToTable("InterfaceBandwidth");
            builder.Entity<PortBandwidth>().ToTable("PortBandwidth");
            builder.Entity<Port>().ToTable("Port");
            builder.Entity<MultiPort>().ToTable("MultiPort");
            builder.Entity<MultiPortPort>().ToTable("MultiPortPort");
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

            // Prevent cascade deletes

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

            builder.Entity<BundleInterfacePort>()
                   .HasOne(c => c.Port)
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
                   .HasOne(c => c.InterfaceBandwidth)
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

            builder.Entity<Vpn>()
                   .HasOne(c => c.Tenant)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.Tenant)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Vrf>()
                   .HasOne(c => c.Tenant)
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

            // Ports which are members of a bundle interface must be unique

            builder.Entity<BundleInterfacePort>()
           .HasIndex(p => new { p.PortID }).IsUnique();

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

            // Vlans which are created under an Interface or Bundle Interface must be unique

            builder.Entity<InterfaceVlan>()
            .HasIndex(p => new { p.InterfaceID, p.VlanTag }).IsUnique();

            // Device names must be unique

            builder.Entity<Device>()
            .HasIndex(p => p.Name).IsUnique();

            // Location names must be unique

            builder.Entity<Location>()
            .HasIndex(p => p.SiteName).IsUnique();

            // Interface bandwidth options must be unique

            builder.Entity<InterfaceBandwidth>()
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