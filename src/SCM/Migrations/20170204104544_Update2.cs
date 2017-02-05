using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantNetworkBgpPeer");

            migrationBuilder.CreateTable(
                name: "BgpPeerTenantNetwork",
                columns: table => new
                {
                    BgpPeerTenantNetworkID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BgpPeerID = table.Column<int>(nullable: false),
                    TenantNetworkID = table.Column<int>(nullable: false),
                    Weight = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BgpPeerTenantNetwork", x => x.BgpPeerTenantNetworkID);
                    table.ForeignKey(
                        name: "FK_BgpPeerTenantNetwork_BgpPeer_BgpPeerID",
                        column: x => x.BgpPeerID,
                        principalTable: "BgpPeer",
                        principalColumn: "BgpPeerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BgpPeerTenantNetwork_TenantNetwork_TenantNetworkID",
                        column: x => x.TenantNetworkID,
                        principalTable: "TenantNetwork",
                        principalColumn: "TenantNetworkID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeerTenantNetwork_TenantNetworkID",
                table: "BgpPeerTenantNetwork",
                column: "TenantNetworkID");

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeerTenantNetwork_BgpPeerID_TenantNetworkID",
                table: "BgpPeerTenantNetwork",
                columns: new[] { "BgpPeerID", "TenantNetworkID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BgpPeerTenantNetwork");

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

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetworkBgpPeer_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                column: "TenantNetworkID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetworkBgpPeer_BgpPeerID_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                columns: new[] { "BgpPeerID", "TenantNetworkID" },
                unique: true);
        }
    }
}
