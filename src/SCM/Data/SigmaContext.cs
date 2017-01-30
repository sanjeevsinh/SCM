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
        public DbSet<AttachmentSetVpn> AttachmentSetVpns { get; set; }
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
        public DbSet<InterfaceBandwidth> LogicalBandwidth { get; set; }
        public DbSet<BundleInterface> BundleInterfaces { get; set; }
        public DbSet<BundleInterfacePort> BundleInterfacePorts { get; set; }
        public DbSet<InterfaceVlan> InterfaceVlans { get; set; }
        public DbSet<BundleInterfaceVlan> BundleInterfaceVlans { get; set; }
        public DbSet<Vrf> Vrfs { get; set; }
        public DbSet<BgpPeer> BgpPeers { get; set; }
        public DbSet<Tenant> Tenant { get; set; }
        public DbSet<TenantNetwork> TenantNetworks { get; set; }
        public DbSet<TenantNetworkBgpPeer> TenantNetworkBgpPeers { get; set; }
        public DbSet<Plane> Planes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<AttachmentSet>().ToTable("AttachmentSet");
            builder.Entity<AttachmentSetVpn>().ToTable("AttachmentSetVpn");
            builder.Entity<AttachmentSetVrf>().ToTable("AttachmentSetVrf");
            builder.Entity<BgpPeer>().ToTable("BgpPeer");
            builder.Entity<BundleInterface>().ToTable("BundleInterface");
            builder.Entity<BundleInterfacePort>().ToTable("BundleInterfacePort");
            builder.Entity<BundleInterfaceVlan>().ToTable("BundleInterfaceVlan");
            builder.Entity<ContractBandwidth>().ToTable("ContractBandwidth");
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
            builder.Entity<TenantNetworkBgpPeer>().ToTable("TenantNetworkBgpPeer");
            builder.Entity<Vpn>().ToTable("Vpn");
            builder.Entity<VpnProtocolType>().ToTable("VpnProtocolType");
            builder.Entity<VpnTenancyType>().ToTable("VpnTenancyType");
            builder.Entity<VpnTopologyType>().ToTable("VpnTopologyType");
            builder.Entity<Plane>().ToTable("Plane");

            // Set Indexes

            builder.Entity<BundleInterfacePort>()
           .HasIndex(p => new { p.PortID }).IsUnique();

            builder.Entity<VpnProtocolType>()
            .HasIndex(p => new { p.ProtocolType }).IsUnique();

            builder.Entity<VpnTenancyType>()
            .HasIndex(p => new { p.TenancyType }).IsUnique();

            builder.Entity<VpnTopologyType>()
            .HasIndex(p => new { p.TopologyType, p.VpnProtocolTypeID }).IsUnique();

            builder.Entity<Vrf>()
            .HasIndex(p => new { p.AdministratorSubField, p.AssignedNumberSubField }).IsUnique();

            builder.Entity<InterfaceVlan>()
            .HasIndex(p => new { p.InterfaceID, p.VlanTag }).IsUnique();

            builder.Entity<BundleInterfaceVlan>()
            .HasIndex(p => new { p.BundleInterfaceID, p.VlanTag }).IsUnique();

            builder.Entity<Device>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<Location>()
            .HasIndex(p => p.SiteName).IsUnique();

            builder.Entity<InterfaceBandwidth>()
            .HasIndex(p => p.BandwidthKbps).IsUnique();

            builder.Entity<PortBandwidth>()
            .HasIndex(p => p.BandwidthKbps).IsUnique();

            builder.Entity<Port>()
            .HasIndex(p => new { p.Type, p.Name, p.DeviceID }).IsUnique();

            builder.Entity<Region>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<SubRegion>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<RouteTarget>()
            .HasIndex(p => new { p.AdministratorSubField, p.AssignedNumberSubField }).IsUnique();

            builder.Entity<AttachmentSetVrf>()
            .HasKey(p => new { p.AttachmentSetID, p.VrfID });

            builder.Entity<AttachmentSetVpn>()
            .HasKey(p => new { p.AttachmentSetID, p.VpnID });

            builder.Entity<Tenant>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<TenantNetworkBgpPeer>()
            .HasIndex(p => new { p.BgpPeerID, p.TenantNetworkID }).IsUnique();

            builder.Entity<Plane>()
            .HasIndex(p => p.Name).IsUnique();

            builder.Entity<VpnTenantNetwork>()
            .HasIndex(p => new { p.TenantNetworkID, p.VpnID }).IsUnique();

            builder.Entity<BundleInterface>()
            .HasIndex(p => new { p.DeviceID, p.ID }).IsUnique();

            // Prevent cascade deletes

            builder.Entity<AttachmentSet>()
                   .HasOne(c => c.SubRegion)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSetVrf>()
                   .HasOne(c => c.Vrf)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AttachmentSetVpn>()
                   .HasOne(c => c.Vpn)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Port>()
                   .HasOne(c => c.PortBandwidth)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Interface>()
                   .HasOne(c => c.InterfaceBandwidth)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BundleInterface>()
                   .HasOne(c => c.InterfaceBandwidth)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InterfaceVlan>()
                   .HasOne(c => c.Vrf)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Location>()
                   .HasOne(c => c.AlternateLocation)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BundleInterface>()
                   .HasOne(c => c.Device)
                   .WithMany()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}