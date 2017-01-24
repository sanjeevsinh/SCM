using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SCM.Data;

namespace SCM.Migrations
{
    [DbContext(typeof(SigmaContext))]
    [Migration("20170122183512_Update16")]
    partial class Update16
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SCM.Models.BgpPeer", b =>
                {
                    b.Property<int>("BgpPeerID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AutonomousSystem");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int?>("VrfID");

                    b.HasKey("BgpPeerID");

                    b.HasIndex("VrfID");

                    b.ToTable("BgpPeer");
                });

            modelBuilder.Entity("SCM.Models.BundleInterface", b =>
                {
                    b.Property<int>("BundleInterfaceID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<bool>("IsTagged");

                    b.Property<int>("LogicalBandwidthID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int?>("VrfID");

                    b.HasKey("BundleInterfaceID");

                    b.HasIndex("LogicalBandwidthID");

                    b.HasIndex("VrfID");

                    b.ToTable("BundleInterface");
                });

            modelBuilder.Entity("SCM.Models.BundleInterfacePort", b =>
                {
                    b.Property<int>("BundleInterfacePortID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BundleInterfaceID");

                    b.Property<int>("PortID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("BundleInterfacePortID");

                    b.HasIndex("PortID");

                    b.HasIndex("BundleInterfaceID", "PortID")
                        .IsUnique();

                    b.ToTable("BundleInterfacePort");
                });

            modelBuilder.Entity("SCM.Models.BundleInterfaceVlan", b =>
                {
                    b.Property<int>("BundleInterfaceVlanID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BundleInterfaceID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<int>("LogicalBandwidthID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int>("VlanID");

                    b.Property<int?>("VrfID");

                    b.HasKey("BundleInterfaceVlanID");

                    b.HasIndex("LogicalBandwidthID");

                    b.HasIndex("VlanID");

                    b.HasIndex("VrfID");

                    b.HasIndex("BundleInterfaceID", "VlanID")
                        .IsUnique();

                    b.ToTable("BundleInterfaceVlan");
                });

            modelBuilder.Entity("SCM.Models.Device", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<int>("LocationID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PlaneID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ID");

                    b.HasIndex("LocationID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PlaneID");

                    b.ToTable("Device");
                });

            modelBuilder.Entity("SCM.Models.Interface", b =>
                {
                    b.Property<int>("ID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<bool>("IsTagged");

                    b.Property<int>("LogicalBandwidthID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int?>("VrfID");

                    b.HasKey("ID");

                    b.HasIndex("LogicalBandwidthID");

                    b.HasIndex("VrfID");

                    b.ToTable("Interface");
                });

            modelBuilder.Entity("SCM.Models.InterfaceBandwidth", b =>
                {
                    b.Property<int>("InterfaceBandwidthID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BandwidthKbps");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("InterfaceBandwidthID");

                    b.HasIndex("BandwidthKbps")
                        .IsUnique();

                    b.ToTable("LogicalBandwidth");
                });

            modelBuilder.Entity("SCM.Models.InterfaceVlan", b =>
                {
                    b.Property<int>("InterfaceVlanID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("InterfaceID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<int>("LogicalBandwidthID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int>("VlanID");

                    b.Property<int?>("VrfID");

                    b.HasKey("InterfaceVlanID");

                    b.HasIndex("LogicalBandwidthID");

                    b.HasIndex("VlanID");

                    b.HasIndex("VrfID");

                    b.HasIndex("InterfaceID", "VlanID")
                        .IsUnique();

                    b.ToTable("InterfaceVlan");
                });

            modelBuilder.Entity("SCM.Models.Location", b =>
                {
                    b.Property<int>("LocationID")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("AlternateLocationLocationID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SiteName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("SubRegionID");

                    b.Property<int>("Tier");

                    b.HasKey("LocationID");

                    b.HasIndex("AlternateLocationLocationID");

                    b.HasIndex("SiteName")
                        .IsUnique();

                    b.HasIndex("SubRegionID");

                    b.ToTable("Location");
                });

            modelBuilder.Entity("SCM.Models.Plane", b =>
                {
                    b.Property<int>("PlaneID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("PlaneID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Plane");
                });

            modelBuilder.Entity("SCM.Models.Port", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DeviceID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("PhysicalPortBandwidthID");

                    b.Property<int?>("PortBandwidthID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int?>("TenantID");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("PortBandwidthID");

                    b.HasIndex("TenantID");

                    b.HasIndex("Type", "Name", "DeviceID")
                        .IsUnique();

                    b.ToTable("Port");
                });

            modelBuilder.Entity("SCM.Models.PortBandwidth", b =>
                {
                    b.Property<int>("PortBandwidthID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BandwidthKbps");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("PortBandwidthID");

                    b.HasIndex("BandwidthKbps")
                        .IsUnique();

                    b.ToTable("PhysicalPortBandwidth");
                });

            modelBuilder.Entity("SCM.Models.Region", b =>
                {
                    b.Property<int>("RegionID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("RegionID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Region");
                });

            modelBuilder.Entity("SCM.Models.RouteTarget", b =>
                {
                    b.Property<int>("RouteTargetID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubField");

                    b.Property<bool>("IsHubExport");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VpnID");

                    b.HasKey("RouteTargetID");

                    b.HasIndex("VpnID");

                    b.HasIndex("AdministratorSubField", "AssignedNumberSubField")
                        .IsUnique();

                    b.ToTable("RouteTarget");
                });

            modelBuilder.Entity("SCM.Models.SubRegion", b =>
                {
                    b.Property<int>("SubRegionID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("RegionID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("SubRegionID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("RegionID");

                    b.ToTable("SubRegion");
                });

            modelBuilder.Entity("SCM.Models.Tenant", b =>
                {
                    b.Property<int>("TenantID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("TenantID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("SCM.Models.TenantNetwork", b =>
                {
                    b.Property<int>("TenantNetworkID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BgpPeerID");

                    b.Property<string>("IpPrefix")
                        .HasMaxLength(15);

                    b.Property<int>("Length");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VpnID");

                    b.HasKey("TenantNetworkID");

                    b.HasIndex("BgpPeerID");

                    b.HasIndex("VpnID");

                    b.ToTable("TenantNetwork");
                });

            modelBuilder.Entity("SCM.Models.Vlan", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TagID");

                    b.HasKey("ID");

                    b.ToTable("Vlan");
                });

            modelBuilder.Entity("SCM.Models.Vpn", b =>
                {
                    b.Property<int>("VpnID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<bool>("ForceAssistedVpnAttachment");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int?>("PlaneID");

                    b.Property<int?>("RegionID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.Property<int>("VpnTenancyTypeID");

                    b.Property<int>("VpnTopologyTypeID");

                    b.HasKey("VpnID");

                    b.HasIndex("PlaneID");

                    b.HasIndex("RegionID");

                    b.HasIndex("TenantID");

                    b.HasIndex("VpnTenancyTypeID");

                    b.HasIndex("VpnTopologyTypeID");

                    b.ToTable("Vpn");
                });

            modelBuilder.Entity("SCM.Models.VpnProtocolType", b =>
                {
                    b.Property<int>("VpnProtocolTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ProtocolType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("VpnProtocolTypeID");

                    b.HasIndex("ProtocolType")
                        .IsUnique();

                    b.ToTable("VpnProtocolType");
                });

            modelBuilder.Entity("SCM.Models.VpnTenancyType", b =>
                {
                    b.Property<int>("VpnTenancyTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("TenancyType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("VpnTenancyTypeID");

                    b.HasIndex("TenancyType")
                        .IsUnique();

                    b.ToTable("VpnTenancyType");
                });

            modelBuilder.Entity("SCM.Models.VpnTopologyType", b =>
                {
                    b.Property<int>("VpnTopologyTypeID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("TopologyType")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("VpnProtocolTypeID");

                    b.HasKey("VpnTopologyTypeID");

                    b.HasIndex("VpnProtocolTypeID");

                    b.HasIndex("TopologyType", "VpnProtocolTypeID")
                        .IsUnique();

                    b.ToTable("VpnTopologyType");
                });

            modelBuilder.Entity("SCM.Models.VpnVrf", b =>
                {
                    b.Property<int>("VpnID");

                    b.Property<int>("VrfID");

                    b.Property<int?>("VpnID1");

                    b.Property<int?>("VrfID1");

                    b.HasKey("VpnID", "VrfID");

                    b.HasIndex("VpnID1");

                    b.HasIndex("VrfID");

                    b.HasIndex("VrfID1");

                    b.ToTable("VpnVrf");
                });

            modelBuilder.Entity("SCM.Models.Vrf", b =>
                {
                    b.Property<int>("VrfID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubField");

                    b.Property<int>("DeviceID");

                    b.Property<string>("Name");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.HasKey("VrfID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("TenantID");

                    b.HasIndex("AdministratorSubField", "AssignedNumberSubField")
                        .IsUnique();

                    b.ToTable("Vrfs");
                });

            modelBuilder.Entity("SCM.Models.BgpPeer", b =>
                {
                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("BgpPeers")
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.BundleInterface", b =>
                {
                    b.HasOne("SCM.Models.InterfaceBandwidth", "LogicalBandwidth")
                        .WithMany()
                        .HasForeignKey("LogicalBandwidthID");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.BundleInterfacePort", b =>
                {
                    b.HasOne("SCM.Models.BundleInterface", "BundleInterface")
                        .WithMany("BundleInterfacePort")
                        .HasForeignKey("BundleInterfaceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Port", "Port")
                        .WithMany("BundleInterfacePort")
                        .HasForeignKey("PortID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.BundleInterfaceVlan", b =>
                {
                    b.HasOne("SCM.Models.BundleInterface", "BundleInterface")
                        .WithMany("BundleInterfaceVlans")
                        .HasForeignKey("BundleInterfaceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.InterfaceBandwidth", "LogicalBandwidth")
                        .WithMany()
                        .HasForeignKey("LogicalBandwidthID");

                    b.HasOne("SCM.Models.Vlan", "Vlan")
                        .WithMany()
                        .HasForeignKey("VlanID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.Device", b =>
                {
                    b.HasOne("SCM.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Plane", "Plane")
                        .WithMany()
                        .HasForeignKey("PlaneID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.Interface", b =>
                {
                    b.HasOne("SCM.Models.Port", "Port")
                        .WithOne("Interface")
                        .HasForeignKey("SCM.Models.Interface", "ID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.InterfaceBandwidth", "LogicalBandwidth")
                        .WithMany()
                        .HasForeignKey("LogicalBandwidthID");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.InterfaceVlan", b =>
                {
                    b.HasOne("SCM.Models.Interface", "Interface")
                        .WithMany()
                        .HasForeignKey("InterfaceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.InterfaceBandwidth", "LogicalBandwidth")
                        .WithMany()
                        .HasForeignKey("LogicalBandwidthID");

                    b.HasOne("SCM.Models.Vlan", "Vlan")
                        .WithMany()
                        .HasForeignKey("VlanID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.Location", b =>
                {
                    b.HasOne("SCM.Models.Location", "AlternateLocation")
                        .WithMany()
                        .HasForeignKey("AlternateLocationLocationID");

                    b.HasOne("SCM.Models.SubRegion", "SubRegion")
                        .WithMany("Locations")
                        .HasForeignKey("SubRegionID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.Port", b =>
                {
                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany("Ports")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.PortBandwidth", "PortBandwidth")
                        .WithMany()
                        .HasForeignKey("PortBandwidthID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");
                });

            modelBuilder.Entity("SCM.Models.RouteTarget", b =>
                {
                    b.HasOne("SCM.Models.Vpn", "Vpn")
                        .WithMany("RouteTargets")
                        .HasForeignKey("VpnID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.SubRegion", b =>
                {
                    b.HasOne("SCM.Models.Region", "Region")
                        .WithMany("SubRegions")
                        .HasForeignKey("RegionID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.TenantNetwork", b =>
                {
                    b.HasOne("SCM.Models.BgpPeer", "BgpPeer")
                        .WithMany()
                        .HasForeignKey("BgpPeerID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vpn", "Vpn")
                        .WithMany("TenantNetworks")
                        .HasForeignKey("VpnID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.Vpn", b =>
                {
                    b.HasOne("SCM.Models.Plane", "Plane")
                        .WithMany()
                        .HasForeignKey("PlaneID");

                    b.HasOne("SCM.Models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VpnTenancyType", "VpnTenancyType")
                        .WithMany("Vpns")
                        .HasForeignKey("VpnTenancyTypeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VpnTopologyType", "VpnTopologyType")
                        .WithMany("Vpns")
                        .HasForeignKey("VpnTopologyTypeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.VpnTopologyType", b =>
                {
                    b.HasOne("SCM.Models.VpnProtocolType", "VpnProtocolType")
                        .WithMany("VpnTopologyTypes")
                        .HasForeignKey("VpnProtocolTypeID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.VpnVrf", b =>
                {
                    b.HasOne("SCM.Models.Vpn", "Vpn")
                        .WithMany()
                        .HasForeignKey("VpnID");

                    b.HasOne("SCM.Models.Vpn")
                        .WithMany("VpnVrfs")
                        .HasForeignKey("VpnID1");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");

                    b.HasOne("SCM.Models.Vrf")
                        .WithMany("VpnVrfs")
                        .HasForeignKey("VrfID1");
                });

            modelBuilder.Entity("SCM.Models.Vrf", b =>
                {
                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany("Vrfs")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
