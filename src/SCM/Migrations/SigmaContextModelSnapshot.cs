using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SCM.Data;

namespace SCM.Migrations
{
    [DbContext(typeof(SigmaContext))]
    partial class SigmaContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SCM.Models.AttachmentRedundancy", b =>
                {
                    b.Property<int>("AttachmentRedundancyID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("AttachmentRedundancyID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("AttachmentRedundancy");
                });

            modelBuilder.Entity("SCM.Models.AttachmentSet", b =>
                {
                    b.Property<int>("AttachmentSetID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentRedundancyID");

                    b.Property<int>("ContractBandwidthID");

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("RegionID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int?>("SubRegionID");

                    b.Property<int>("TenantID");

                    b.HasKey("AttachmentSetID");

                    b.HasIndex("AttachmentRedundancyID");

                    b.HasIndex("ContractBandwidthID");

                    b.HasIndex("RegionID");

                    b.HasIndex("SubRegionID");

                    b.HasIndex("TenantID");

                    b.ToTable("AttachmentSet");
                });

            modelBuilder.Entity("SCM.Models.AttachmentSetVrf", b =>
                {
                    b.Property<int>("AttachmentSetVrfID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentSetID");

                    b.Property<int?>("Preference");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VrfID");

                    b.HasKey("AttachmentSetVrfID");

                    b.HasIndex("VrfID");

                    b.HasIndex("AttachmentSetID", "VrfID")
                        .IsUnique();

                    b.ToTable("AttachmentSetVrf");
                });

            modelBuilder.Entity("SCM.Models.BgpPeer", b =>
                {
                    b.Property<int>("BgpPeerID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AutonomousSystem");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<int?>("MaximumRoutes");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VrfID");

                    b.HasKey("BgpPeerID");

                    b.HasIndex("VrfID", "IpAddress")
                        .IsUnique();

                    b.ToTable("BgpPeer");
                });

            modelBuilder.Entity("SCM.Models.BundleInterface", b =>
                {
                    b.Property<int>("BundleInterfaceID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DeviceID");

                    b.Property<int>("ID");

                    b.Property<int>("InterfaceBandwidthID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<bool>("IsLayer3");

                    b.Property<bool>("IsTagged");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int?>("VrfID");

                    b.HasKey("BundleInterfaceID");

                    b.HasIndex("InterfaceBandwidthID");

                    b.HasIndex("VrfID");

                    b.HasIndex("DeviceID", "ID")
                        .IsUnique();

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

                    b.HasIndex("BundleInterfaceID");

                    b.HasIndex("PortID")
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

                    b.Property<bool>("IsLayer3");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int>("VlanTag");

                    b.Property<int>("VrfID");

                    b.HasKey("BundleInterfaceVlanID");

                    b.HasIndex("VrfID");

                    b.HasIndex("BundleInterfaceID", "VlanTag")
                        .IsUnique();

                    b.ToTable("BundleInterfaceVlan");
                });

            modelBuilder.Entity("SCM.Models.ContractBandwidth", b =>
                {
                    b.Property<int>("ContractBandwidthID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BandwidthKbps");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ContractBandwidthID");

                    b.HasIndex("BandwidthKbps")
                        .IsUnique();

                    b.ToTable("ContractBandwidth");
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

                    b.Property<int>("InterfaceBandwidthID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<bool>("IsLayer3");

                    b.Property<bool>("IsTagged");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int?>("VrfID");

                    b.HasKey("ID");

                    b.HasIndex("InterfaceBandwidthID");

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

                    b.ToTable("InterfaceBandwidth");
                });

            modelBuilder.Entity("SCM.Models.InterfaceVlan", b =>
                {
                    b.Property<int>("InterfaceVlanID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("InterfaceID");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<bool>("IsLayer3");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int>("VlanTag");

                    b.Property<int>("VrfID");

                    b.HasKey("InterfaceVlanID");

                    b.HasIndex("VrfID");

                    b.HasIndex("InterfaceID", "VlanTag")
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

            modelBuilder.Entity("SCM.Models.MultiPort", b =>
                {
                    b.Property<int>("MultiPortID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("MultiPortID");

                    b.ToTable("MultiPort");
                });

            modelBuilder.Entity("SCM.Models.MultiPortPort", b =>
                {
                    b.Property<int>("MultiPortPortID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("MultiPortID");

                    b.Property<int>("PortID");

                    b.HasKey("MultiPortPortID");

                    b.HasIndex("MultiPortID");

                    b.HasIndex("PortID");

                    b.ToTable("MultiPortPort");
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

                    b.Property<int>("PortBandwidthID");

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

                    b.ToTable("PortBandwidth");
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

                    b.Property<string>("AdministratorSubField");

                    b.Property<string>("AssignedNumberSubField");

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

                    b.Property<string>("IpPrefix")
                        .HasMaxLength(15);

                    b.Property<int>("Length");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.HasKey("TenantNetworkID");

                    b.HasIndex("TenantID", "IpPrefix", "Length")
                        .IsUnique();

                    b.ToTable("TenantNetwork");
                });

            modelBuilder.Entity("SCM.Models.Vpn", b =>
                {
                    b.Property<int>("VpnID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<bool>("IsExtranet");

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

            modelBuilder.Entity("SCM.Models.VpnAttachmentSet", b =>
                {
                    b.Property<int>("VpnAttachmentSetID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentSetID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VpnID");

                    b.HasKey("VpnAttachmentSetID");

                    b.HasIndex("VpnID");

                    b.HasIndex("AttachmentSetID", "VpnID")
                        .IsUnique();

                    b.ToTable("VpnAttachmentSet");
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

            modelBuilder.Entity("SCM.Models.VpnTenantNetwork", b =>
                {
                    b.Property<int>("VpnTenantNetworkID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantNetworkID");

                    b.Property<int>("VpnAttachmentSetID");

                    b.HasKey("VpnTenantNetworkID");

                    b.HasIndex("VpnAttachmentSetID");

                    b.HasIndex("TenantNetworkID", "VpnAttachmentSetID")
                        .IsUnique();

                    b.ToTable("VpnTenantNetwork");
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

            modelBuilder.Entity("SCM.Models.Vrf", b =>
                {
                    b.Property<int>("VrfID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AdministratorSubField")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("AssignedNumberSubField")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<int>("DeviceID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

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

            modelBuilder.Entity("SCM.Models.AttachmentSet", b =>
                {
                    b.HasOne("SCM.Models.AttachmentRedundancy", "AttachmentRedundancy")
                        .WithMany()
                        .HasForeignKey("AttachmentRedundancyID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.ContractBandwidth", "ContractBandwidth")
                        .WithMany()
                        .HasForeignKey("ContractBandwidthID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.SubRegion", "SubRegion")
                        .WithMany()
                        .HasForeignKey("SubRegionID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.AttachmentSetVrf", b =>
                {
                    b.HasOne("SCM.Models.AttachmentSet", "AttachmentSet")
                        .WithMany("AttachmentSetVrfs")
                        .HasForeignKey("AttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.BgpPeer", b =>
                {
                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("BgpPeers")
                        .HasForeignKey("VrfID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.BundleInterface", b =>
                {
                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceID");

                    b.HasOne("SCM.Models.InterfaceBandwidth", "InterfaceBandwidth")
                        .WithMany()
                        .HasForeignKey("InterfaceBandwidthID");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.BundleInterfacePort", b =>
                {
                    b.HasOne("SCM.Models.BundleInterface", "BundleInterface")
                        .WithMany("BundleInterfacePorts")
                        .HasForeignKey("BundleInterfaceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Port", "Port")
                        .WithOne("BundleInterfacePort")
                        .HasForeignKey("SCM.Models.BundleInterfacePort", "PortID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.BundleInterfaceVlan", b =>
                {
                    b.HasOne("SCM.Models.BundleInterface", "BundleInterface")
                        .WithMany("BundleInterfaceVlans")
                        .HasForeignKey("BundleInterfaceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID")
                        .OnDelete(DeleteBehavior.Cascade);
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

                    b.HasOne("SCM.Models.InterfaceBandwidth", "InterfaceBandwidth")
                        .WithMany()
                        .HasForeignKey("InterfaceBandwidthID");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany()
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.InterfaceVlan", b =>
                {
                    b.HasOne("SCM.Models.Interface", "Interface")
                        .WithMany("InterfaceVlans")
                        .HasForeignKey("InterfaceID")
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
                        .WithMany()
                        .HasForeignKey("SubRegionID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.MultiPortPort", b =>
                {
                    b.HasOne("SCM.Models.MultiPort", "MultiPort")
                        .WithMany("MultiPortPorts")
                        .HasForeignKey("MultiPortID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Port", "Port")
                        .WithMany()
                        .HasForeignKey("PortID")
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
                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID")
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

            modelBuilder.Entity("SCM.Models.VpnAttachmentSet", b =>
                {
                    b.HasOne("SCM.Models.AttachmentSet", "AttachmentSet")
                        .WithMany()
                        .HasForeignKey("AttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vpn", "Vpn")
                        .WithMany()
                        .HasForeignKey("VpnID");
                });

            modelBuilder.Entity("SCM.Models.VpnTenantNetwork", b =>
                {
                    b.HasOne("SCM.Models.TenantNetwork", "TenantNetwork")
                        .WithMany()
                        .HasForeignKey("TenantNetworkID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VpnAttachmentSet", "VpnAttachmentSet")
                        .WithMany()
                        .HasForeignKey("VpnAttachmentSetID");
                });

            modelBuilder.Entity("SCM.Models.VpnTopologyType", b =>
                {
                    b.HasOne("SCM.Models.VpnProtocolType", "VpnProtocolType")
                        .WithMany("VpnTopologyTypes")
                        .HasForeignKey("VpnProtocolTypeID")
                        .OnDelete(DeleteBehavior.Cascade);
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
