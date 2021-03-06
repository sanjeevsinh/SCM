﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttachmentBandwidth",
                columns: table => new
                {
                    AttachmentBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthGbps = table.Column<int>(nullable: false),
                    BundleOrMultiPortMemberBandwidthGbps = table.Column<int>(nullable: true),
                    MustBeBundleOrMultiPort = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SupportedByBundle = table.Column<bool>(nullable: false),
                    SupportedByMultiPort = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentBandwidth", x => x.AttachmentBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentRedundancy",
                columns: table => new
                {
                    AttachmentRedundancyID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentRedundancy", x => x.AttachmentRedundancyID);
                });

            migrationBuilder.CreateTable(
                name: "ContractBandwidth",
                columns: table => new
                {
                    ContractBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthMbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractBandwidth", x => x.ContractBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "Plane",
                columns: table => new
                {
                    PlaneID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plane", x => x.PlaneID);
                });

            migrationBuilder.CreateTable(
                name: "PortBandwidth",
                columns: table => new
                {
                    PortBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthGbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortBandwidth", x => x.PortBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "Region",
                columns: table => new
                {
                    RegionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Region", x => x.RegionID);
                });

            migrationBuilder.CreateTable(
                name: "RouteDistinguisherRange",
                columns: table => new
                {
                    RouteDistinguisherRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldCount = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldStart = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteDistinguisherRange", x => x.RouteDistinguisherRangeID);
                });

            migrationBuilder.CreateTable(
                name: "RouteTargetRange",
                columns: table => new
                {
                    RouteTargetRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldCount = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldStart = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTargetRange", x => x.RouteTargetRangeID);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    TenantID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantID);
                });

            migrationBuilder.CreateTable(
                name: "VlanTagRange",
                columns: table => new
                {
                    VlanTagRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VlanTagRangeCount = table.Column<int>(nullable: false),
                    VlanTagRangeStart = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VlanTagRange", x => x.VlanTagRangeID);
                });

            migrationBuilder.CreateTable(
                name: "VpnProtocolType",
                columns: table => new
                {
                    VpnProtocolTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProtocolType = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnProtocolType", x => x.VpnProtocolTypeID);
                });

            migrationBuilder.CreateTable(
                name: "VpnTenancyType",
                columns: table => new
                {
                    VpnTenancyTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenancyType = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTenancyType", x => x.VpnTenancyTypeID);
                });

            migrationBuilder.CreateTable(
                name: "SubRegion",
                columns: table => new
                {
                    SubRegionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RegionID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubRegion", x => x.SubRegionID);
                    table.ForeignKey(
                        name: "FK_SubRegion_Region_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Region",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContractBandwidthPool",
                columns: table => new
                {
                    ContractBandwidthPoolID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractBandwidthID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    TrustReceivedCosDscp = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractBandwidthPool", x => x.ContractBandwidthPoolID);
                    table.ForeignKey(
                        name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                        column: x => x.ContractBandwidthID,
                        principalTable: "ContractBandwidth",
                        principalColumn: "ContractBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContractBandwidthPool_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantCommunity",
                columns: table => new
                {
                    TenantCommunityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowExtranet = table.Column<bool>(nullable: false),
                    AutonomousSystemNumber = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCommunity", x => x.TenantCommunityID);
                    table.ForeignKey(
                        name: "FK_TenantCommunity_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantNetwork",
                columns: table => new
                {
                    TenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AllowExtranet = table.Column<bool>(nullable: false),
                    IpPrefix = table.Column<string>(maxLength: 15, nullable: true),
                    Length = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNetwork", x => x.TenantNetworkID);
                    table.ForeignKey(
                        name: "FK_TenantNetwork_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VpnTopologyType",
                columns: table => new
                {
                    VpnTopologyTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TopologyType = table.Column<string>(maxLength: 50, nullable: false),
                    VpnProtocolTypeID = table.Column<int>(nullable: false),
                    VpnProtocolTypeID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTopologyType", x => x.VpnTopologyTypeID);
                    table.ForeignKey(
                        name: "FK_VpnTopologyType_VpnProtocolType_VpnProtocolTypeID",
                        column: x => x.VpnProtocolTypeID,
                        principalTable: "VpnProtocolType",
                        principalColumn: "VpnProtocolTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VpnTopologyType_VpnProtocolType_VpnProtocolTypeID1",
                        column: x => x.VpnProtocolTypeID1,
                        principalTable: "VpnProtocolType",
                        principalColumn: "VpnProtocolTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentSet",
                columns: table => new
                {
                    AttachmentSetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentRedundancyID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RegionID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubRegionID = table.Column<int>(nullable: true),
                    TenantID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentSet", x => x.AttachmentSetID);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_AttachmentRedundancy_AttachmentRedundancyID",
                        column: x => x.AttachmentRedundancyID,
                        principalTable: "AttachmentRedundancy",
                        principalColumn: "AttachmentRedundancyID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_Region_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Region",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_SubRegion_SubRegionID",
                        column: x => x.SubRegionID,
                        principalTable: "SubRegion",
                        principalColumn: "SubRegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateLocationLocationID = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SiteName = table.Column<string>(maxLength: 50, nullable: false),
                    SubRegionID = table.Column<int>(nullable: false),
                    Tier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                    table.ForeignKey(
                        name: "FK_Location_Location_AlternateLocationLocationID",
                        column: x => x.AlternateLocationLocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Location_SubRegion_SubRegionID",
                        column: x => x.SubRegionID,
                        principalTable: "SubRegion",
                        principalColumn: "SubRegionID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vpn",
                columns: table => new
                {
                    VpnID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    IsExtranet = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PlaneID = table.Column<int>(nullable: true),
                    RegionID = table.Column<int>(nullable: true),
                    RequiresSync = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    VpnTenancyTypeID = table.Column<int>(nullable: false),
                    VpnTenancyTypeID1 = table.Column<int>(nullable: true),
                    VpnTopologyTypeID = table.Column<int>(nullable: false),
                    VpnTopologyTypeID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vpn", x => x.VpnID);
                    table.ForeignKey(
                        name: "FK_Vpn_Plane_PlaneID",
                        column: x => x.PlaneID,
                        principalTable: "Plane",
                        principalColumn: "PlaneID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_Region_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Region",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTenancyType_VpnTenancyTypeID",
                        column: x => x.VpnTenancyTypeID,
                        principalTable: "VpnTenancyType",
                        principalColumn: "VpnTenancyTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTenancyType_VpnTenancyTypeID1",
                        column: x => x.VpnTenancyTypeID1,
                        principalTable: "VpnTenancyType",
                        principalColumn: "VpnTenancyTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTopologyType_VpnTopologyTypeID",
                        column: x => x.VpnTopologyTypeID,
                        principalTable: "VpnTopologyType",
                        principalColumn: "VpnTopologyTypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTopologyType_VpnTopologyTypeID1",
                        column: x => x.VpnTopologyTypeID1,
                        principalTable: "VpnTopologyType",
                        principalColumn: "VpnTopologyTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    LocationID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PlaneID = table.Column<int>(nullable: false),
                    RequiresSync = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Device_Location_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Location",
                        principalColumn: "LocationID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Device_Plane_PlaneID",
                        column: x => x.PlaneID,
                        principalTable: "Plane",
                        principalColumn: "PlaneID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RouteTarget",
                columns: table => new
                {
                    RouteTargetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubField = table.Column<int>(nullable: false),
                    IsHubExport = table.Column<bool>(nullable: false),
                    RouteTargetRangeID = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTarget", x => x.RouteTargetID);
                    table.ForeignKey(
                        name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                        column: x => x.RouteTargetRangeID,
                        principalTable: "RouteTargetRange",
                        principalColumn: "RouteTargetRangeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RouteTarget_Vpn_VpnID",
                        column: x => x.VpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VpnAttachmentSet",
                columns: table => new
                {
                    VpnAttachmentSetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentSetID = table.Column<int>(nullable: false),
                    IsHub = table.Column<bool>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnAttachmentSet", x => x.VpnAttachmentSetID);
                    table.ForeignKey(
                        name: "FK_VpnAttachmentSet_AttachmentSet_AttachmentSetID",
                        column: x => x.AttachmentSetID,
                        principalTable: "AttachmentSet",
                        principalColumn: "AttachmentSetID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VpnAttachmentSet_Vpn_VpnID",
                        column: x => x.VpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vrf",
                columns: table => new
                {
                    VrfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubField = table.Column<int>(nullable: false),
                    DeviceID = table.Column<int>(nullable: false),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RouteDistinguisherRangeID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vrf", x => x.VrfID);
                    table.ForeignKey(
                        name: "FK_Vrf_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID",
                        column: x => x.RouteDistinguisherRangeID,
                        principalTable: "RouteDistinguisherRange",
                        principalColumn: "RouteDistinguisherRangeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vrf_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VpnTenantCommunity",
                columns: table => new
                {
                    VpnTenantCommunityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantCommunityID = table.Column<int>(nullable: false),
                    VpnAttachmentSetID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTenantCommunity", x => x.VpnTenantCommunityID);
                    table.ForeignKey(
                        name: "FK_VpnTenantCommunity_TenantCommunity_TenantCommunityID",
                        column: x => x.TenantCommunityID,
                        principalTable: "TenantCommunity",
                        principalColumn: "TenantCommunityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VpnTenantCommunity_VpnAttachmentSet_VpnAttachmentSetID",
                        column: x => x.VpnAttachmentSetID,
                        principalTable: "VpnAttachmentSet",
                        principalColumn: "VpnAttachmentSetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VpnTenantNetwork",
                columns: table => new
                {
                    VpnTenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantNetworkID = table.Column<int>(nullable: false),
                    VpnAttachmentSetID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTenantNetwork", x => x.VpnTenantNetworkID);
                    table.ForeignKey(
                        name: "FK_VpnTenantNetwork_TenantNetwork_TenantNetworkID",
                        column: x => x.TenantNetworkID,
                        principalTable: "TenantNetwork",
                        principalColumn: "TenantNetworkID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VpnTenantNetwork_VpnAttachmentSet_VpnAttachmentSetID",
                        column: x => x.VpnAttachmentSetID,
                        principalTable: "VpnAttachmentSet",
                        principalColumn: "VpnAttachmentSetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    AttachmentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentBandwidthID = table.Column<int>(nullable: false),
                    ContractBandwidthPoolID = table.Column<int>(nullable: false),
                    ContractBandwidthPoolID1 = table.Column<int>(nullable: true),
                    DeviceID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: true),
                    IsBundle = table.Column<bool>(nullable: false),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    IsMultiPort = table.Column<bool>(nullable: false),
                    IsTagged = table.Column<bool>(nullable: false),
                    RequiresSync = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    TenantID1 = table.Column<int>(nullable: true),
                    VrfID = table.Column<int>(nullable: false),
                    VrfID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.AttachmentID);
                    table.ForeignKey(
                        name: "FK_Attachment_AttachmentBandwidth_AttachmentBandwidthID",
                        column: x => x.AttachmentBandwidthID,
                        principalTable: "AttachmentBandwidth",
                        principalColumn: "AttachmentBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                        column: x => x.ContractBandwidthPoolID,
                        principalTable: "ContractBandwidthPool",
                        principalColumn: "ContractBandwidthPoolID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID1",
                        column: x => x.ContractBandwidthPoolID1,
                        principalTable: "ContractBandwidthPool",
                        principalColumn: "ContractBandwidthPoolID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachment_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_Tenant_TenantID1",
                        column: x => x.TenantID1,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_Vrf_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attachment_Vrf_VrfID1",
                        column: x => x.VrfID1,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentSetVrf",
                columns: table => new
                {
                    AttachmentSetVrfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentSetID = table.Column<int>(nullable: false),
                    Preference = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VrfID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentSetVrf", x => x.AttachmentSetVrfID);
                    table.ForeignKey(
                        name: "FK_AttachmentSetVrf_AttachmentSet_AttachmentSetID",
                        column: x => x.AttachmentSetID,
                        principalTable: "AttachmentSet",
                        principalColumn: "AttachmentSetID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachmentSetVrf_Vrf_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BgpPeer",
                columns: table => new
                {
                    BgpPeerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutonomousSystem = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsBfdEnabled = table.Column<bool>(nullable: false),
                    MaximumRoutes = table.Column<int>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VrfID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BgpPeer", x => x.BgpPeerID);
                    table.ForeignKey(
                        name: "FK_BgpPeer_Vrf_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interface",
                columns: table => new
                {
                    InterfaceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentID = table.Column<int>(nullable: false),
                    DeviceID = table.Column<int>(nullable: false),
                    DeviceID1 = table.Column<int>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interface", x => x.InterfaceID);
                    table.ForeignKey(
                        name: "FK_Interface_Attachment_AttachmentID",
                        column: x => x.AttachmentID,
                        principalTable: "Attachment",
                        principalColumn: "AttachmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interface_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interface_Device_DeviceID1",
                        column: x => x.DeviceID1,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vif",
                columns: table => new
                {
                    VifID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentID = table.Column<int>(nullable: false),
                    ContractBandwidthPoolID = table.Column<int>(nullable: false),
                    ContractBandwidthPoolID1 = table.Column<int>(nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    RequiresSync = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    TenantID1 = table.Column<int>(nullable: true),
                    VlanTag = table.Column<int>(nullable: false),
                    VlanTagRangeID = table.Column<int>(nullable: true),
                    VrfID = table.Column<int>(nullable: false),
                    VrfID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vif", x => x.VifID);
                    table.ForeignKey(
                        name: "FK_Vif_Attachment_AttachmentID",
                        column: x => x.AttachmentID,
                        principalTable: "Attachment",
                        principalColumn: "AttachmentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                        column: x => x.ContractBandwidthPoolID,
                        principalTable: "ContractBandwidthPool",
                        principalColumn: "ContractBandwidthPoolID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID1",
                        column: x => x.ContractBandwidthPoolID1,
                        principalTable: "ContractBandwidthPool",
                        principalColumn: "ContractBandwidthPoolID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_Tenant_TenantID1",
                        column: x => x.TenantID1,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_VlanTagRange_VlanTagRangeID",
                        column: x => x.VlanTagRangeID,
                        principalTable: "VlanTagRange",
                        principalColumn: "VlanTagRangeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_Vrf_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vif_Vrf_VrfID1",
                        column: x => x.VrfID1,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Port",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceID = table.Column<int>(nullable: false),
                    InterfaceID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PortBandwidthID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: true),
                    Type = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Port", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Port_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Port_Interface_InterfaceID",
                        column: x => x.InterfaceID,
                        principalTable: "Interface",
                        principalColumn: "InterfaceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Port_PortBandwidth_PortBandwidthID",
                        column: x => x.PortBandwidthID,
                        principalTable: "PortBandwidth",
                        principalColumn: "PortBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Port_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vlan",
                columns: table => new
                {
                    VlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InterfaceID = table.Column<int>(nullable: false),
                    InterfaceID1 = table.Column<int>(nullable: true),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VifID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vlan", x => x.VlanID);
                    table.ForeignKey(
                        name: "FK_Vlan_Interface_InterfaceID",
                        column: x => x.InterfaceID,
                        principalTable: "Interface",
                        principalColumn: "InterfaceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vlan_Interface_InterfaceID1",
                        column: x => x.InterfaceID1,
                        principalTable: "Interface",
                        principalColumn: "InterfaceID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vlan_Vif_VifID",
                        column: x => x.VifID,
                        principalTable: "Vif",
                        principalColumn: "VifID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_AttachmentBandwidthID",
                table: "Attachment",
                column: "AttachmentBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_ContractBandwidthPoolID1",
                table: "Attachment",
                column: "ContractBandwidthPoolID1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_DeviceID",
                table: "Attachment",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TenantID",
                table: "Attachment",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_TenantID1",
                table: "Attachment",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_VrfID",
                table: "Attachment",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_VrfID1",
                table: "Attachment",
                column: "VrfID1");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentBandwidth_BandwidthGbps",
                table: "AttachmentBandwidth",
                column: "BandwidthGbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentRedundancy_Name",
                table: "AttachmentRedundancy",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_AttachmentRedundancyID",
                table: "AttachmentSet",
                column: "AttachmentRedundancyID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_Name",
                table: "AttachmentSet",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_RegionID",
                table: "AttachmentSet",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_SubRegionID",
                table: "AttachmentSet",
                column: "SubRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_TenantID",
                table: "AttachmentSet",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSetVrf_VrfID",
                table: "AttachmentSetVrf",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSetVrf_AttachmentSetID_VrfID",
                table: "AttachmentSetVrf",
                columns: new[] { "AttachmentSetID", "VrfID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeer_VrfID_IpAddress",
                table: "BgpPeer",
                columns: new[] { "VrfID", "IpAddress" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidth_BandwidthMbps",
                table: "ContractBandwidth",
                column: "BandwidthMbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidthPool_ContractBandwidthID",
                table: "ContractBandwidthPool",
                column: "ContractBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidthPool_Name",
                table: "ContractBandwidthPool",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidthPool_TenantID",
                table: "ContractBandwidthPool",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Device_LocationID",
                table: "Device",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Device_Name",
                table: "Device",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Device_PlaneID",
                table: "Device",
                column: "PlaneID");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_AttachmentID",
                table: "Interface",
                column: "AttachmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_DeviceID",
                table: "Interface",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_DeviceID1",
                table: "Interface",
                column: "DeviceID1");

            migrationBuilder.CreateIndex(
                name: "IX_Location_AlternateLocationLocationID",
                table: "Location",
                column: "AlternateLocationLocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Location_SiteName",
                table: "Location",
                column: "SiteName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_SubRegionID",
                table: "Location",
                column: "SubRegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Plane_Name",
                table: "Plane",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_DeviceID",
                table: "Port",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_InterfaceID",
                table: "Port",
                column: "InterfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_PortBandwidthID",
                table: "Port",
                column: "PortBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_TenantID",
                table: "Port",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_Type_Name_DeviceID",
                table: "Port",
                columns: new[] { "Type", "Name", "DeviceID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortBandwidth_BandwidthGbps",
                table: "PortBandwidth",
                column: "BandwidthGbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_Name",
                table: "Region",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_VpnID",
                table: "RouteTarget",
                column: "VpnID");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_AdministratorSubField_AssignedNumberSubField",
                table: "RouteTarget",
                columns: new[] { "AdministratorSubField", "AssignedNumberSubField" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubRegion_Name",
                table: "SubRegion",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubRegion_RegionID",
                table: "SubRegion",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Tenant_Name",
                table: "Tenant",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_TenantID",
                table: "TenantCommunity",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_AutonomousSystemNumber_Number",
                table: "TenantCommunity",
                columns: new[] { "AutonomousSystemNumber", "Number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_TenantID",
                table: "TenantNetwork",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_IpPrefix_Length",
                table: "TenantNetwork",
                columns: new[] { "IpPrefix", "Length" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vif_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_ContractBandwidthPoolID1",
                table: "Vif",
                column: "ContractBandwidthPoolID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_TenantID",
                table: "Vif",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_TenantID1",
                table: "Vif",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_VlanTagRangeID",
                table: "Vif",
                column: "VlanTagRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_VrfID",
                table: "Vif",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_VrfID1",
                table: "Vif",
                column: "VrfID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_AttachmentID_VlanTag",
                table: "Vif",
                columns: new[] { "AttachmentID", "VlanTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vlan_InterfaceID",
                table: "Vlan",
                column: "InterfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_Vlan_InterfaceID1",
                table: "Vlan",
                column: "InterfaceID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vlan_VifID",
                table: "Vlan",
                column: "VifID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_Name",
                table: "Vpn",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_PlaneID",
                table: "Vpn",
                column: "PlaneID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_RegionID",
                table: "Vpn",
                column: "RegionID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_TenantID",
                table: "Vpn",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_VpnTenancyTypeID",
                table: "Vpn",
                column: "VpnTenancyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_VpnTenancyTypeID1",
                table: "Vpn",
                column: "VpnTenancyTypeID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_VpnTopologyTypeID",
                table: "Vpn",
                column: "VpnTopologyTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_VpnTopologyTypeID1",
                table: "Vpn",
                column: "VpnTopologyTypeID1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnAttachmentSet_VpnID",
                table: "VpnAttachmentSet",
                column: "VpnID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnAttachmentSet_AttachmentSetID_VpnID",
                table: "VpnAttachmentSet",
                columns: new[] { "AttachmentSetID", "VpnID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnProtocolType_ProtocolType",
                table: "VpnProtocolType",
                column: "ProtocolType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenancyType_TenancyType",
                table: "VpnTenancyType",
                column: "TenancyType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantCommunity_TenantCommunityID",
                table: "VpnTenantCommunity",
                column: "TenantCommunityID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantCommunity_VpnAttachmentSetID",
                table: "VpnTenantCommunity",
                column: "VpnAttachmentSetID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_VpnAttachmentSetID",
                table: "VpnTenantNetwork",
                column: "VpnAttachmentSetID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnAttachmentSetID",
                table: "VpnTenantNetwork",
                columns: new[] { "TenantNetworkID", "VpnAttachmentSetID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                column: "VpnProtocolTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_VpnProtocolTypeID1",
                table: "VpnTopologyType",
                column: "VpnProtocolTypeID1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_TopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                columns: new[] { "TopologyType", "VpnProtocolTypeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_DeviceID",
                table: "Vrf",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_Name",
                table: "Vrf",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID",
                table: "Vrf",
                column: "RouteDistinguisherRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_TenantID",
                table: "Vrf",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_AdministratorSubField_AssignedNumberSubField",
                table: "Vrf",
                columns: new[] { "AdministratorSubField", "AssignedNumberSubField" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentSetVrf");

            migrationBuilder.DropTable(
                name: "BgpPeer");

            migrationBuilder.DropTable(
                name: "Port");

            migrationBuilder.DropTable(
                name: "RouteTarget");

            migrationBuilder.DropTable(
                name: "Vlan");

            migrationBuilder.DropTable(
                name: "VpnTenantCommunity");

            migrationBuilder.DropTable(
                name: "VpnTenantNetwork");

            migrationBuilder.DropTable(
                name: "PortBandwidth");

            migrationBuilder.DropTable(
                name: "RouteTargetRange");

            migrationBuilder.DropTable(
                name: "Interface");

            migrationBuilder.DropTable(
                name: "Vif");

            migrationBuilder.DropTable(
                name: "TenantCommunity");

            migrationBuilder.DropTable(
                name: "TenantNetwork");

            migrationBuilder.DropTable(
                name: "VpnAttachmentSet");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "VlanTagRange");

            migrationBuilder.DropTable(
                name: "AttachmentSet");

            migrationBuilder.DropTable(
                name: "Vpn");

            migrationBuilder.DropTable(
                name: "AttachmentBandwidth");

            migrationBuilder.DropTable(
                name: "ContractBandwidthPool");

            migrationBuilder.DropTable(
                name: "Vrf");

            migrationBuilder.DropTable(
                name: "AttachmentRedundancy");

            migrationBuilder.DropTable(
                name: "VpnTenancyType");

            migrationBuilder.DropTable(
                name: "VpnTopologyType");

            migrationBuilder.DropTable(
                name: "ContractBandwidth");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "RouteDistinguisherRange");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "VpnProtocolType");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "Plane");

            migrationBuilder.DropTable(
                name: "SubRegion");

            migrationBuilder.DropTable(
                name: "Region");
        }
    }
}
