﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SCM.Data;

namespace SCM.Migrations
{
    [DbContext(typeof(SigmaContext))]
    [Migration("20170425214057_Update8")]
    partial class Update8
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SCM.Models.Attachment", b =>
                {
                    b.Property<int>("AttachmentID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentBandwidthID");

                    b.Property<int?>("ContractBandwidthPoolID");

                    b.Property<int>("DeviceID");

                    b.Property<int?>("ID");

                    b.Property<bool>("IsBundle");

                    b.Property<bool>("IsLayer3");

                    b.Property<bool>("IsMultiPort");

                    b.Property<bool>("IsTagged");

                    b.Property<bool>("RequiresSync");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.Property<int?>("TenantID1");

                    b.Property<int?>("VrfID");

                    b.HasKey("AttachmentID");

                    b.HasIndex("AttachmentBandwidthID");

                    b.HasIndex("ContractBandwidthPoolID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("TenantID");

                    b.HasIndex("TenantID1");

                    b.HasIndex("VrfID");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("SCM.Models.AttachmentBandwidth", b =>
                {
                    b.Property<int>("AttachmentBandwidthID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BandwidthGbps");

                    b.Property<int?>("BundleOrMultiPortMemberBandwidthGbps");

                    b.Property<bool>("MustBeBundleOrMultiPort");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<bool>("SupportedByBundle");

                    b.Property<bool>("SupportedByMultiPort");

                    b.HasKey("AttachmentBandwidthID");

                    b.HasIndex("BandwidthGbps")
                        .IsUnique();

                    b.ToTable("AttachmentBandwidth");
                });

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

                    b.Property<string>("Description")
                        .HasMaxLength(250);

                    b.Property<bool>("IsLayer3");

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

                    b.HasIndex("Name")
                        .IsUnique();

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

                    b.Property<bool>("IsBfdEnabled");

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

            modelBuilder.Entity("SCM.Models.ContractBandwidth", b =>
                {
                    b.Property<int>("ContractBandwidthID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BandwidthMbps");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("ContractBandwidthID");

                    b.HasIndex("BandwidthMbps")
                        .IsUnique();

                    b.ToTable("ContractBandwidth");
                });

            modelBuilder.Entity("SCM.Models.ContractBandwidthPool", b =>
                {
                    b.Property<int>("ContractBandwidthPoolID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("ContractBandwidthID");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.Property<bool>("TrustReceivedCosDscp");

                    b.HasKey("ContractBandwidthPoolID");

                    b.HasIndex("ContractBandwidthID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("TenantID");

                    b.ToTable("ContractBandwidthPool");
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

                    b.Property<bool>("RequiresSync");

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
                    b.Property<int>("InterfaceID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentID");

                    b.Property<int>("DeviceID");

                    b.Property<int?>("DeviceID1");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.HasKey("InterfaceID");

                    b.HasIndex("AttachmentID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("DeviceID1");

                    b.ToTable("Interface");
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

                    b.Property<int?>("InterfaceID");

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

                    b.HasIndex("InterfaceID");

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

                    b.Property<int>("BandwidthGbps");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("PortBandwidthID");

                    b.HasIndex("BandwidthGbps")
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

            modelBuilder.Entity("SCM.Models.RouteDistinguisherRange", b =>
                {
                    b.Property<int>("RouteDistinguisherRangeID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubFieldCount");

                    b.Property<int>("AssignedNumberSubFieldStart");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("RouteDistinguisherRangeID");

                    b.ToTable("RouteDistinguisherRange");
                });

            modelBuilder.Entity("SCM.Models.RouteTarget", b =>
                {
                    b.Property<int>("RouteTargetID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubField");

                    b.Property<bool>("IsHubExport");

                    b.Property<int?>("RouteTargetRangeID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VpnID");

                    b.HasKey("RouteTargetID");

                    b.HasIndex("RouteTargetRangeID");

                    b.HasIndex("VpnID");

                    b.HasIndex("AdministratorSubField", "AssignedNumberSubField")
                        .IsUnique();

                    b.ToTable("RouteTarget");
                });

            modelBuilder.Entity("SCM.Models.RouteTargetRange", b =>
                {
                    b.Property<int>("RouteTargetRangeID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubFieldCount");

                    b.Property<int>("AssignedNumberSubFieldStart");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.HasKey("RouteTargetRangeID");

                    b.ToTable("RouteTargetRange");
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

            modelBuilder.Entity("SCM.Models.TenantCommunity", b =>
                {
                    b.Property<int>("TenantCommunityID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowExtranet");

                    b.Property<int>("AutonomousSystemNumber");

                    b.Property<int>("Number");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.HasKey("TenantCommunityID");

                    b.HasIndex("TenantID");

                    b.HasIndex("AutonomousSystemNumber", "Number")
                        .IsUnique();

                    b.ToTable("TenantCommunity");
                });

            modelBuilder.Entity("SCM.Models.TenantNetwork", b =>
                {
                    b.Property<int>("TenantNetworkID")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowExtranet");

                    b.Property<string>("IpPrefix")
                        .HasMaxLength(15);

                    b.Property<int>("Length");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.HasKey("TenantNetworkID");

                    b.HasIndex("TenantID");

                    b.HasIndex("IpPrefix", "Length")
                        .IsUnique();

                    b.ToTable("TenantNetwork");
                });

            modelBuilder.Entity("SCM.Models.Vif", b =>
                {
                    b.Property<int>("VifID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentID");

                    b.Property<int?>("ContractBandwidthPoolID");

                    b.Property<bool>("IsLayer3");

                    b.Property<bool>("RequiresSync");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.Property<int>("VlanTag");

                    b.Property<int?>("VlanTagRangeID");

                    b.Property<int?>("VrfID");

                    b.HasKey("VifID");

                    b.HasIndex("ContractBandwidthPoolID");

                    b.HasIndex("TenantID");

                    b.HasIndex("VlanTagRangeID");

                    b.HasIndex("VrfID");

                    b.HasIndex("AttachmentID", "VlanTag")
                        .IsUnique();

                    b.ToTable("Vif");
                });

            modelBuilder.Entity("SCM.Models.Vlan", b =>
                {
                    b.Property<int>("VlanID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("InterfaceID");

                    b.Property<int?>("InterfaceID1");

                    b.Property<string>("IpAddress")
                        .HasMaxLength(15);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("SubnetMask")
                        .HasMaxLength(15);

                    b.Property<int>("VifID");

                    b.HasKey("VlanID");

                    b.HasIndex("InterfaceID");

                    b.HasIndex("InterfaceID1");

                    b.HasIndex("VifID");

                    b.ToTable("Vlan");
                });

            modelBuilder.Entity("SCM.Models.VlanTagRange", b =>
                {
                    b.Property<int>("VlanTagRangeID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("VlanTagRangeCount");

                    b.Property<int>("VlanTagRangeStart");

                    b.HasKey("VlanTagRangeID");

                    b.ToTable("VlanTagRange");
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

                    b.Property<bool>("RequiresSync");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.Property<int>("VpnTenancyTypeID");

                    b.Property<int?>("VpnTenancyTypeID1");

                    b.Property<int>("VpnTopologyTypeID");

                    b.Property<int?>("VpnTopologyTypeID1");

                    b.HasKey("VpnID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("PlaneID");

                    b.HasIndex("RegionID");

                    b.HasIndex("TenantID");

                    b.HasIndex("VpnTenancyTypeID");

                    b.HasIndex("VpnTenancyTypeID1");

                    b.HasIndex("VpnTopologyTypeID");

                    b.HasIndex("VpnTopologyTypeID1");

                    b.ToTable("Vpn");
                });

            modelBuilder.Entity("SCM.Models.VpnAttachmentSet", b =>
                {
                    b.Property<int>("VpnAttachmentSetID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AttachmentSetID");

                    b.Property<bool?>("IsHub");

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

            modelBuilder.Entity("SCM.Models.VpnTenantCommunity", b =>
                {
                    b.Property<int>("VpnTenantCommunityID")
                        .ValueGeneratedOnAdd();

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantCommunityID");

                    b.Property<int>("VpnAttachmentSetID");

                    b.HasKey("VpnTenantCommunityID");

                    b.HasIndex("TenantCommunityID");

                    b.HasIndex("VpnAttachmentSetID");

                    b.ToTable("VpnTenantCommunity");
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

                    b.Property<int?>("VpnProtocolTypeID1");

                    b.HasKey("VpnTopologyTypeID");

                    b.HasIndex("VpnProtocolTypeID");

                    b.HasIndex("VpnProtocolTypeID1");

                    b.HasIndex("TopologyType", "VpnProtocolTypeID")
                        .IsUnique();

                    b.ToTable("VpnTopologyType");
                });

            modelBuilder.Entity("SCM.Models.Vrf", b =>
                {
                    b.Property<int>("VrfID")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AdministratorSubField");

                    b.Property<int>("AssignedNumberSubField");

                    b.Property<int>("DeviceID");

                    b.Property<bool>("IsLayer3");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<int>("RouteDistinguisherRangeID");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<int>("TenantID");

                    b.HasKey("VrfID");

                    b.HasIndex("DeviceID");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.HasIndex("RouteDistinguisherRangeID");

                    b.HasIndex("TenantID");

                    b.HasIndex("AdministratorSubField", "AssignedNumberSubField")
                        .IsUnique();

                    b.ToTable("Vrf");
                });

            modelBuilder.Entity("SCM.Models.Attachment", b =>
                {
                    b.HasOne("SCM.Models.AttachmentBandwidth", "AttachmentBandwidth")
                        .WithMany()
                        .HasForeignKey("AttachmentBandwidthID");

                    b.HasOne("SCM.Models.ContractBandwidthPool", "ContractBandwidthPool")
                        .WithMany("Attachments")
                        .HasForeignKey("ContractBandwidthPoolID");

                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany("Attachments")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");

                    b.HasOne("SCM.Models.Tenant")
                        .WithMany("Attachments")
                        .HasForeignKey("TenantID1");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("Attachments")
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.AttachmentSet", b =>
                {
                    b.HasOne("SCM.Models.AttachmentRedundancy", "AttachmentRedundancy")
                        .WithMany()
                        .HasForeignKey("AttachmentRedundancyID");

                    b.HasOne("SCM.Models.Region", "Region")
                        .WithMany()
                        .HasForeignKey("RegionID");

                    b.HasOne("SCM.Models.SubRegion", "SubRegion")
                        .WithMany()
                        .HasForeignKey("SubRegionID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");
                });

            modelBuilder.Entity("SCM.Models.AttachmentSetVrf", b =>
                {
                    b.HasOne("SCM.Models.AttachmentSet", "AttachmentSet")
                        .WithMany("AttachmentSetVrfs")
                        .HasForeignKey("AttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("AttachmentSetVrfs")
                        .HasForeignKey("VrfID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.BgpPeer", b =>
                {
                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("BgpPeers")
                        .HasForeignKey("VrfID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.ContractBandwidthPool", b =>
                {
                    b.HasOne("SCM.Models.ContractBandwidth", "ContractBandwidth")
                        .WithMany()
                        .HasForeignKey("ContractBandwidthID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.Device", b =>
                {
                    b.HasOne("SCM.Models.Location", "Location")
                        .WithMany()
                        .HasForeignKey("LocationID");

                    b.HasOne("SCM.Models.Plane", "Plane")
                        .WithMany()
                        .HasForeignKey("PlaneID");
                });

            modelBuilder.Entity("SCM.Models.Interface", b =>
                {
                    b.HasOne("SCM.Models.Attachment", "Attachment")
                        .WithMany("Interfaces")
                        .HasForeignKey("AttachmentID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceID");

                    b.HasOne("SCM.Models.Device")
                        .WithMany("Interfaces")
                        .HasForeignKey("DeviceID1");
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

            modelBuilder.Entity("SCM.Models.Port", b =>
                {
                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany("Ports")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Interface", "Interface")
                        .WithMany("Ports")
                        .HasForeignKey("InterfaceID");

                    b.HasOne("SCM.Models.PortBandwidth", "PortBandwidth")
                        .WithMany()
                        .HasForeignKey("PortBandwidthID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany("Ports")
                        .HasForeignKey("TenantID");
                });

            modelBuilder.Entity("SCM.Models.RouteTarget", b =>
                {
                    b.HasOne("SCM.Models.RouteTargetRange", "RouteTargetRange")
                        .WithMany("RouteTargets")
                        .HasForeignKey("RouteTargetRangeID");

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

            modelBuilder.Entity("SCM.Models.TenantCommunity", b =>
                {
                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany("TenantCommunities")
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.TenantNetwork", b =>
                {
                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany("TenantNetworks")
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.Vif", b =>
                {
                    b.HasOne("SCM.Models.Attachment", "Attachment")
                        .WithMany("Vifs")
                        .HasForeignKey("AttachmentID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.ContractBandwidthPool", "ContractBandwidthPool")
                        .WithMany("Vifs")
                        .HasForeignKey("ContractBandwidthPoolID");

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany("Vifs")
                        .HasForeignKey("TenantID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VlanTagRange", "VlanTagRange")
                        .WithMany("Vifs")
                        .HasForeignKey("VlanTagRangeID");

                    b.HasOne("SCM.Models.Vrf", "Vrf")
                        .WithMany("Vifs")
                        .HasForeignKey("VrfID");
                });

            modelBuilder.Entity("SCM.Models.Vlan", b =>
                {
                    b.HasOne("SCM.Models.Interface", "Interface")
                        .WithMany()
                        .HasForeignKey("InterfaceID");

                    b.HasOne("SCM.Models.Interface")
                        .WithMany("Vlans")
                        .HasForeignKey("InterfaceID1");

                    b.HasOne("SCM.Models.Vif", "Vif")
                        .WithMany("Vlans")
                        .HasForeignKey("VifID")
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
                        .HasForeignKey("TenantID");

                    b.HasOne("SCM.Models.VpnTenancyType", "VpnTenancyType")
                        .WithMany()
                        .HasForeignKey("VpnTenancyTypeID");

                    b.HasOne("SCM.Models.VpnTenancyType")
                        .WithMany("Vpns")
                        .HasForeignKey("VpnTenancyTypeID1");

                    b.HasOne("SCM.Models.VpnTopologyType", "VpnTopologyType")
                        .WithMany()
                        .HasForeignKey("VpnTopologyTypeID");

                    b.HasOne("SCM.Models.VpnTopologyType")
                        .WithMany("Vpns")
                        .HasForeignKey("VpnTopologyTypeID1");
                });

            modelBuilder.Entity("SCM.Models.VpnAttachmentSet", b =>
                {
                    b.HasOne("SCM.Models.AttachmentSet", "AttachmentSet")
                        .WithMany("VpnAttachmentSets")
                        .HasForeignKey("AttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Vpn", "Vpn")
                        .WithMany("VpnAttachmentSets")
                        .HasForeignKey("VpnID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.VpnTenantCommunity", b =>
                {
                    b.HasOne("SCM.Models.TenantCommunity", "TenantCommunity")
                        .WithMany()
                        .HasForeignKey("TenantCommunityID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VpnAttachmentSet", "VpnAttachmentSet")
                        .WithMany("VpnTenantCommunities")
                        .HasForeignKey("VpnAttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.VpnTenantNetwork", b =>
                {
                    b.HasOne("SCM.Models.TenantNetwork", "TenantNetwork")
                        .WithMany()
                        .HasForeignKey("TenantNetworkID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.VpnAttachmentSet", "VpnAttachmentSet")
                        .WithMany("VpnTenantNetworks")
                        .HasForeignKey("VpnAttachmentSetID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SCM.Models.VpnTopologyType", b =>
                {
                    b.HasOne("SCM.Models.VpnProtocolType", "VpnProtocolType")
                        .WithMany()
                        .HasForeignKey("VpnProtocolTypeID");

                    b.HasOne("SCM.Models.VpnProtocolType")
                        .WithMany("VpnTopologyTypes")
                        .HasForeignKey("VpnProtocolTypeID1");
                });

            modelBuilder.Entity("SCM.Models.Vrf", b =>
                {
                    b.HasOne("SCM.Models.Device", "Device")
                        .WithMany("Vrfs")
                        .HasForeignKey("DeviceID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.RouteDistinguisherRange", "RouteDistinguisherRange")
                        .WithMany("Vrfs")
                        .HasForeignKey("RouteDistinguisherRangeID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("SCM.Models.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantID");
                });
        }
    }
}
