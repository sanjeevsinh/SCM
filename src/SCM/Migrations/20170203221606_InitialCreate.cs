using System;
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
                    BandwidthKbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractBandwidth", x => x.ContractBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "InterfaceBandwidth",
                columns: table => new
                {
                    InterfaceBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthKbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterfaceBandwidth", x => x.InterfaceBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "MultiPort",
                columns: table => new
                {
                    MultiPortID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPort", x => x.MultiPortID);
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
                    BandwidthKbps = table.Column<int>(nullable: false),
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
                name: "TenantNetwork",
                columns: table => new
                {
                    TenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IpPrefix = table.Column<string>(maxLength: 15, nullable: true),
                    Length = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantNetworkVpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNetwork", x => x.TenantNetworkID);
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
                name: "VpnTopologyType",
                columns: table => new
                {
                    VpnTopologyTypeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TopologyType = table.Column<string>(maxLength: 50, nullable: false),
                    VpnProtocolTypeID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTopologyType", x => x.VpnTopologyTypeID);
                    table.ForeignKey(
                        name: "FK_VpnTopologyType_VpnProtocolType_VpnProtocolTypeID",
                        column: x => x.VpnProtocolTypeID,
                        principalTable: "VpnProtocolType",
                        principalColumn: "VpnProtocolTypeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentSet",
                columns: table => new
                {
                    AttachmentSetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentRedundancyID = table.Column<int>(nullable: false),
                    ContractBandwidthID = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_ContractBandwidth_ContractBandwidthID",
                        column: x => x.ContractBandwidthID,
                        principalTable: "ContractBandwidth",
                        principalColumn: "ContractBandwidthID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttachmentSet_Region_RegionID",
                        column: x => x.RegionID,
                        principalTable: "Region",
                        principalColumn: "RegionID",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
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
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    VpnTenancyTypeID = table.Column<int>(nullable: false),
                    VpnTopologyTypeID = table.Column<int>(nullable: false)
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTenancyType_VpnTenancyTypeID",
                        column: x => x.VpnTenancyTypeID,
                        principalTable: "VpnTenancyType",
                        principalColumn: "VpnTenancyTypeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vpn_VpnTopologyType_VpnTopologyTypeID",
                        column: x => x.VpnTopologyTypeID,
                        principalTable: "VpnTopologyType",
                        principalColumn: "VpnTopologyTypeID",
                        onDelete: ReferentialAction.Cascade);
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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Device_Plane_PlaneID",
                        column: x => x.PlaneID,
                        principalTable: "Plane",
                        principalColumn: "PlaneID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouteTarget",
                columns: table => new
                {
                    RouteTargetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<string>(nullable: true),
                    AssignedNumberSubField = table.Column<string>(nullable: true),
                    IsHubExport = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTarget", x => x.RouteTargetID);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VpnTenantNetwork",
                columns: table => new
                {
                    VpnTenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantNetworkID = table.Column<int>(nullable: false),
                    VpnID = table.Column<int>(nullable: false)
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
                        name: "FK_VpnTenantNetwork_Vpn_VpnID",
                        column: x => x.VpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Port",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceID = table.Column<int>(nullable: false),
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
                name: "Vrfs",
                columns: table => new
                {
                    VrfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<string>(maxLength: 16, nullable: false),
                    AssignedNumberSubField = table.Column<string>(maxLength: 16, nullable: false),
                    DeviceID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vrfs", x => x.VrfID);
                    table.ForeignKey(
                        name: "FK_Vrfs_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vrfs_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MultiPortPort",
                columns: table => new
                {
                    MultiPortPortID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MultiPortID = table.Column<int>(nullable: false),
                    PortID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPortPort", x => x.MultiPortPortID);
                    table.ForeignKey(
                        name: "FK_MultiPortPort_MultiPort_MultiPortID",
                        column: x => x.MultiPortID,
                        principalTable: "MultiPort",
                        principalColumn: "MultiPortID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiPortPort_Port_PortID",
                        column: x => x.PortID,
                        principalTable: "Port",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentSetVrf",
                columns: table => new
                {
                    AttachmentSetVrfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttachmentSetID = table.Column<int>(nullable: false),
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
                        name: "FK_AttachmentSetVrf_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BgpPeer",
                columns: table => new
                {
                    BgpPeerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutonomousSystem = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    MaximumRoutes = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VrfID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BgpPeer", x => x.BgpPeerID);
                    table.ForeignKey(
                        name: "FK_BgpPeer_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BundleInterface",
                columns: table => new
                {
                    BundleInterfaceID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceID = table.Column<int>(nullable: false),
                    ID = table.Column<int>(nullable: false),
                    InterfaceBandwidthID = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    IsTagged = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VrfID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleInterface", x => x.BundleInterfaceID);
                    table.ForeignKey(
                        name: "FK_BundleInterface_Device_DeviceID",
                        column: x => x.DeviceID,
                        principalTable: "Device",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BundleInterface_InterfaceBandwidth_InterfaceBandwidthID",
                        column: x => x.InterfaceBandwidthID,
                        principalTable: "InterfaceBandwidth",
                        principalColumn: "InterfaceBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BundleInterface_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Interface",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false),
                    InterfaceBandwidthID = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    IsTagged = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VrfID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interface", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Interface_Port_ID",
                        column: x => x.ID,
                        principalTable: "Port",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Interface_InterfaceBandwidth_InterfaceBandwidthID",
                        column: x => x.InterfaceBandwidthID,
                        principalTable: "InterfaceBandwidth",
                        principalColumn: "InterfaceBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interface_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantNetworkBgpPeer",
                columns: table => new
                {
                    TenantNetworkBgpPeerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BgpPeerID = table.Column<int>(nullable: false),
                    TenantNetworkID = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNetworkBgpPeer", x => x.TenantNetworkBgpPeerID);
                    table.ForeignKey(
                        name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                        column: x => x.BgpPeerID,
                        principalTable: "BgpPeer",
                        principalColumn: "BgpPeerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                        column: x => x.TenantNetworkID,
                        principalTable: "TenantNetwork",
                        principalColumn: "TenantNetworkID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BundleInterfacePort",
                columns: table => new
                {
                    BundleInterfacePortID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BundleInterfaceID = table.Column<int>(nullable: false),
                    PortID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleInterfacePort", x => x.BundleInterfacePortID);
                    table.ForeignKey(
                        name: "FK_BundleInterfacePort_BundleInterface_BundleInterfaceID",
                        column: x => x.BundleInterfaceID,
                        principalTable: "BundleInterface",
                        principalColumn: "BundleInterfaceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundleInterfacePort_Port_PortID",
                        column: x => x.PortID,
                        principalTable: "Port",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BundleInterfaceVlan",
                columns: table => new
                {
                    BundleInterfaceVlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BundleInterfaceID = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VlanTag = table.Column<int>(nullable: false),
                    VrfID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleInterfaceVlan", x => x.BundleInterfaceVlanID);
                    table.ForeignKey(
                        name: "FK_BundleInterfaceVlan_BundleInterface_BundleInterfaceID",
                        column: x => x.BundleInterfaceID,
                        principalTable: "BundleInterface",
                        principalColumn: "BundleInterfaceID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterfaceVlan",
                columns: table => new
                {
                    InterfaceVlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InterfaceID = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VlanTag = table.Column<int>(nullable: false),
                    VrfID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterfaceVlan", x => x.InterfaceVlanID);
                    table.ForeignKey(
                        name: "FK_InterfaceVlan_Interface_InterfaceID",
                        column: x => x.InterfaceID,
                        principalTable: "Interface",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InterfaceVlan_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_AttachmentSet_ContractBandwidthID",
                table: "AttachmentSet",
                column: "ContractBandwidthID");

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
                name: "IX_BgpPeer_VrfID",
                table: "BgpPeer",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_InterfaceBandwidthID",
                table: "BundleInterface",
                column: "InterfaceBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_VrfID",
                table: "BundleInterface",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_DeviceID_ID",
                table: "BundleInterface",
                columns: new[] { "DeviceID", "ID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_BundleInterfaceID",
                table: "BundleInterfacePort",
                column: "BundleInterfaceID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort",
                column: "PortID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_BundleInterfaceID_VlanTag",
                table: "BundleInterfaceVlan",
                columns: new[] { "BundleInterfaceID", "VlanTag" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidth_BandwidthKbps",
                table: "ContractBandwidth",
                column: "BandwidthKbps",
                unique: true);

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
                name: "IX_Interface_InterfaceBandwidthID",
                table: "Interface",
                column: "InterfaceBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_VrfID",
                table: "Interface",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceBandwidth_BandwidthKbps",
                table: "InterfaceBandwidth",
                column: "BandwidthKbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VrfID",
                table: "InterfaceVlan",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_InterfaceID_VlanTag",
                table: "InterfaceVlan",
                columns: new[] { "InterfaceID", "VlanTag" },
                unique: true);

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
                name: "IX_MultiPortPort_MultiPortID",
                table: "MultiPortPort",
                column: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort",
                column: "PortID");

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
                name: "IX_PortBandwidth_BandwidthKbps",
                table: "PortBandwidth",
                column: "BandwidthKbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Region_Name",
                table: "Region",
                column: "Name",
                unique: true);

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
                name: "IX_TenantNetworkBgpPeer_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                column: "TenantNetworkID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetworkBgpPeer_BgpPeerID_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                columns: new[] { "BgpPeerID", "TenantNetworkID" },
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
                name: "IX_Vpn_VpnTopologyTypeID",
                table: "Vpn",
                column: "VpnTopologyTypeID");

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
                name: "IX_VpnTenantNetwork_VpnID",
                table: "VpnTenantNetwork",
                column: "VpnID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnID",
                table: "VpnTenantNetwork",
                columns: new[] { "TenantNetworkID", "VpnID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                column: "VpnProtocolTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_TopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                columns: new[] { "TopologyType", "VpnProtocolTypeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_DeviceID",
                table: "Vrfs",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_TenantID",
                table: "Vrfs",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_AdministratorSubField_AssignedNumberSubField",
                table: "Vrfs",
                columns: new[] { "AdministratorSubField", "AssignedNumberSubField" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttachmentSetVrf");

            migrationBuilder.DropTable(
                name: "BundleInterfacePort");

            migrationBuilder.DropTable(
                name: "BundleInterfaceVlan");

            migrationBuilder.DropTable(
                name: "InterfaceVlan");

            migrationBuilder.DropTable(
                name: "MultiPortPort");

            migrationBuilder.DropTable(
                name: "RouteTarget");

            migrationBuilder.DropTable(
                name: "TenantNetworkBgpPeer");

            migrationBuilder.DropTable(
                name: "VpnAttachmentSet");

            migrationBuilder.DropTable(
                name: "VpnTenantNetwork");

            migrationBuilder.DropTable(
                name: "BundleInterface");

            migrationBuilder.DropTable(
                name: "Interface");

            migrationBuilder.DropTable(
                name: "MultiPort");

            migrationBuilder.DropTable(
                name: "BgpPeer");

            migrationBuilder.DropTable(
                name: "AttachmentSet");

            migrationBuilder.DropTable(
                name: "TenantNetwork");

            migrationBuilder.DropTable(
                name: "Vpn");

            migrationBuilder.DropTable(
                name: "Port");

            migrationBuilder.DropTable(
                name: "InterfaceBandwidth");

            migrationBuilder.DropTable(
                name: "Vrfs");

            migrationBuilder.DropTable(
                name: "AttachmentRedundancy");

            migrationBuilder.DropTable(
                name: "ContractBandwidth");

            migrationBuilder.DropTable(
                name: "VpnTenancyType");

            migrationBuilder.DropTable(
                name: "VpnTopologyType");

            migrationBuilder.DropTable(
                name: "PortBandwidth");

            migrationBuilder.DropTable(
                name: "Device");

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
