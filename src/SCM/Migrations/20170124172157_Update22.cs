using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PortID",
                table: "Interface");

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
                name: "TenantNetworkBgpPeer",
                columns: table => new
                {
                    TenantNetworkBgpPeerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BgpPeerID = table.Column<int>(nullable: false),
                    TenantNetworkID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantNetworkBgpPeer", x => x.TenantNetworkBgpPeerID);
                    table.ForeignKey(
                        name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                        column: x => x.BgpPeerID,
                        principalTable: "BgpPeer",
                        principalColumn: "BgpPeerID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                        column: x => x.TenantNetworkID,
                        principalTable: "TenantNetwork",
                        principalColumn: "TenantNetworkID",
                        onDelete: ReferentialAction.Restrict);
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

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_MultiPortID",
                table: "MultiPortPort",
                column: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort",
                column: "PortID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetworkBgpPeer_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                column: "TenantNetworkID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetworkBgpPeer_BgpPeerID_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                columns: new[] { "BgpPeerID", "TenantNetworkID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiPortPort");

            migrationBuilder.DropTable(
                name: "TenantNetworkBgpPeer");

            migrationBuilder.DropTable(
                name: "MultiPort");

            migrationBuilder.AddColumn<int>(
                name: "PortID",
                table: "Interface",
                nullable: false,
                defaultValue: 0);
        }
    }
}
