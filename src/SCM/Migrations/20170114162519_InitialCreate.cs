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
                name: "LogicalBandwidth",
                columns: table => new
                {
                    LogicalBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthKbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogicalBandwidth", x => x.LogicalBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalPortBandwidth",
                columns: table => new
                {
                    PhysicalPortBandwidthID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BandwidthKbps = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalPortBandwidth", x => x.PhysicalPortBandwidthID);
                });

            migrationBuilder.CreateTable(
                name: "Plane",
                columns: table => new
                {
                    PlaneID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plane", x => x.PlaneID);
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
                    TenantGuid = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.TenantID);
                });

            migrationBuilder.CreateTable(
                name: "Vlan",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TagID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vlan", x => x.ID);
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
                name: "Location",
                columns: table => new
                {
                    LocationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlternateLocationID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SiteName = table.Column<string>(maxLength: 50, nullable: false),
                    SubRegionID = table.Column<int>(nullable: false),
                    Tier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.LocationID);
                    table.ForeignKey(
                        name: "FK_Location_Location_AlternateLocationID",
                        column: x => x.AlternateLocationID,
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
                    ForceAssistedVpnAttachment = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PlaneID = table.Column<int>(nullable: false),
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
                        onDelete: ReferentialAction.Cascade);
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
                    AutonomousSystem = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    LocationID = table.Column<int>(nullable: false),
                    ManagementIpAddress = table.Column<string>(maxLength: 15, nullable: true),
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
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubField = table.Column<int>(nullable: false),
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
                name: "Port",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    PhysicalPortBandwidthID = table.Column<int>(nullable: false),
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
                        name: "FK_Port_PhysicalPortBandwidth_PhysicalPortBandwidthID",
                        column: x => x.PhysicalPortBandwidthID,
                        principalTable: "PhysicalPortBandwidth",
                        principalColumn: "PhysicalPortBandwidthID",
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
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubField = table.Column<int>(nullable: false),
                    DeviceID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "BgpPeer",
                columns: table => new
                {
                    BgpPeerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutonomousSystem = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
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
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsTagged = table.Column<bool>(nullable: false),
                    LogicalBandwidthID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VrfID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BundleInterface", x => x.BundleInterfaceID);
                    table.ForeignKey(
                        name: "FK_BundleInterface_LogicalBandwidth_LogicalBandwidthID",
                        column: x => x.LogicalBandwidthID,
                        principalTable: "LogicalBandwidth",
                        principalColumn: "LogicalBandwidthID",
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
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    IsTagged = table.Column<bool>(nullable: false),
                    LogicalBandwidthID = table.Column<int>(nullable: false),
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
                        name: "FK_Interface_LogicalBandwidth_LogicalBandwidthID",
                        column: x => x.LogicalBandwidthID,
                        principalTable: "LogicalBandwidth",
                        principalColumn: "LogicalBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interface_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VpnVrf",
                columns: table => new
                {
                    VpnVrfID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    VpnID = table.Column<int>(nullable: false),
                    VpnID1 = table.Column<int>(nullable: true),
                    VrfID = table.Column<int>(nullable: false),
                    VrfID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnVrf", x => x.VpnVrfID);
                    table.ForeignKey(
                        name: "FK_VpnVrf_Vpn_VpnID",
                        column: x => x.VpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VpnVrf_Vpn_VpnID1",
                        column: x => x.VpnID1,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VpnVrf_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VpnVrf_Vrfs_VrfID1",
                        column: x => x.VrfID1,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantNetwork",
                columns: table => new
                {
                    TenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BgpPeerID = table.Column<int>(nullable: false),
                    IpPrefix = table.Column<string>(maxLength: 15, nullable: true),
                    Length = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNetwork", x => x.TenantNetworkID);
                    table.ForeignKey(
                        name: "FK_TenantNetwork_BgpPeer_BgpPeerID",
                        column: x => x.BgpPeerID,
                        principalTable: "BgpPeer",
                        principalColumn: "BgpPeerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantNetwork_Vpn_VpnID",
                        column: x => x.VpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
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
                    LogicalBandwidthID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VlanID = table.Column<int>(nullable: false),
                    VrfID = table.Column<int>(nullable: true)
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
                        name: "FK_BundleInterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                        column: x => x.LogicalBandwidthID,
                        principalTable: "LogicalBandwidth",
                        principalColumn: "LogicalBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BundleInterfaceVlan_Vlan_VlanID",
                        column: x => x.VlanID,
                        principalTable: "Vlan",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrfs",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InterfaceVlan",
                columns: table => new
                {
                    InterfaceVlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    InterfaceID = table.Column<int>(nullable: false),
                    IpAddress = table.Column<string>(maxLength: 15, nullable: true),
                    LogicalBandwidthID = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    SubnetMask = table.Column<string>(maxLength: 15, nullable: true),
                    VlanID = table.Column<int>(nullable: false),
                    VrfID = table.Column<int>(nullable: true)
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
                        name: "FK_InterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                        column: x => x.LogicalBandwidthID,
                        principalTable: "LogicalBandwidth",
                        principalColumn: "LogicalBandwidthID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InterfaceVlan_Vlan_VlanID",
                        column: x => x.VlanID,
                        principalTable: "Vlan",
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
                name: "IX_BgpPeer_VrfID",
                table: "BgpPeer",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_LogicalBandwidthID",
                table: "BundleInterface",
                column: "LogicalBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_VrfID",
                table: "BundleInterface",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort",
                column: "PortID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_BundleInterfaceID_PortID",
                table: "BundleInterfacePort",
                columns: new[] { "BundleInterfaceID", "PortID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_LogicalBandwidthID",
                table: "BundleInterfaceVlan",
                column: "LogicalBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_VlanID",
                table: "BundleInterfaceVlan",
                column: "VlanID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfaceVlan_BundleInterfaceID_VlanID",
                table: "BundleInterfaceVlan",
                columns: new[] { "BundleInterfaceID", "VlanID" },
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
                name: "IX_Interface_LogicalBandwidthID",
                table: "Interface",
                column: "LogicalBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_VrfID",
                table: "Interface",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_LogicalBandwidthID",
                table: "InterfaceVlan",
                column: "LogicalBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VlanID",
                table: "InterfaceVlan",
                column: "VlanID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VrfID",
                table: "InterfaceVlan",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_InterfaceID_VlanID",
                table: "InterfaceVlan",
                columns: new[] { "InterfaceID", "VlanID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_AlternateLocationID",
                table: "Location",
                column: "AlternateLocationID");

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
                name: "IX_LogicalBandwidth_BandwidthKbps",
                table: "LogicalBandwidth",
                column: "BandwidthKbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalPortBandwidth_BandwidthKbps",
                table: "PhysicalPortBandwidth",
                column: "BandwidthKbps",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_DeviceID",
                table: "Port",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_PhysicalPortBandwidthID",
                table: "Port",
                column: "PhysicalPortBandwidthID");

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
                name: "IX_TenantNetwork_BgpPeerID",
                table: "TenantNetwork",
                column: "BgpPeerID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_VpnID",
                table: "TenantNetwork",
                column: "VpnID");

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_PlaneID",
                table: "Vpn",
                column: "PlaneID");

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
                name: "IX_VpnTopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                column: "VpnProtocolTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTopologyType_TopologyType_VpnProtocolTypeID",
                table: "VpnTopologyType",
                columns: new[] { "TopologyType", "VpnProtocolTypeID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VpnID1",
                table: "VpnVrf",
                column: "VpnID1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VrfID",
                table: "VpnVrf",
                column: "VrfID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VrfID1",
                table: "VpnVrf",
                column: "VrfID1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VpnID_VrfID",
                table: "VpnVrf",
                columns: new[] { "VpnID", "VrfID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_DeviceID",
                table: "Vrfs",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_AdministratorSubField_AssignedNumberSubField",
                table: "Vrfs",
                columns: new[] { "AdministratorSubField", "AssignedNumberSubField" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BundleInterfacePort");

            migrationBuilder.DropTable(
                name: "BundleInterfaceVlan");

            migrationBuilder.DropTable(
                name: "InterfaceVlan");

            migrationBuilder.DropTable(
                name: "RouteTarget");

            migrationBuilder.DropTable(
                name: "TenantNetwork");

            migrationBuilder.DropTable(
                name: "VpnVrf");

            migrationBuilder.DropTable(
                name: "BundleInterface");

            migrationBuilder.DropTable(
                name: "Interface");

            migrationBuilder.DropTable(
                name: "Vlan");

            migrationBuilder.DropTable(
                name: "BgpPeer");

            migrationBuilder.DropTable(
                name: "Vpn");

            migrationBuilder.DropTable(
                name: "Port");

            migrationBuilder.DropTable(
                name: "LogicalBandwidth");

            migrationBuilder.DropTable(
                name: "Vrfs");

            migrationBuilder.DropTable(
                name: "VpnTenancyType");

            migrationBuilder.DropTable(
                name: "VpnTopologyType");

            migrationBuilder.DropTable(
                name: "PhysicalPortBandwidth");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Device");

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
