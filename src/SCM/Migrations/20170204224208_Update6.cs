using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropTable(
                name: "BgpPeerTenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnID",
                table: "VpnTenantNetwork");

            migrationBuilder.RenameColumn(
                name: "TenantNetworkVpnID",
                table: "TenantNetwork",
                newName: "TenantID");

            migrationBuilder.AlterColumn<int>(
                name: "VpnID",
                table: "VpnTenantNetwork",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "VpnAttachmentSetID",
                table: "VpnTenantNetwork",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Preference",
                table: "AttachmentSetVrf",
                nullable: true);

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
                name: "IX_TenantNetwork_TenantID",
                table: "TenantNetwork",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VpnTenantNetwork_VpnAttachmentSet_VpnAttachmentSetID",
                table: "VpnTenantNetwork",
                column: "VpnAttachmentSetID",
                principalTable: "VpnAttachmentSet",
                principalColumn: "VpnAttachmentSetID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork",
                column: "VpnID",
                principalTable: "Vpn",
                principalColumn: "VpnID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork");

            migrationBuilder.DropForeignKey(
                name: "FK_VpnTenantNetwork_VpnAttachmentSet_VpnAttachmentSetID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_VpnAttachmentSetID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnAttachmentSetID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_TenantID",
                table: "TenantNetwork");

            migrationBuilder.DropColumn(
                name: "VpnAttachmentSetID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropColumn(
                name: "Preference",
                table: "AttachmentSetVrf");

            migrationBuilder.RenameColumn(
                name: "TenantID",
                table: "TenantNetwork",
                newName: "TenantNetworkVpnID");

            migrationBuilder.AlterColumn<int>(
                name: "VpnID",
                table: "VpnTenantNetwork",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

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
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnID",
                table: "VpnTenantNetwork",
                columns: new[] { "TenantNetworkID", "VpnID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeerTenantNetwork_TenantNetworkID",
                table: "BgpPeerTenantNetwork",
                column: "TenantNetworkID");

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeerTenantNetwork_BgpPeerID_TenantNetworkID",
                table: "BgpPeerTenantNetwork",
                columns: new[] { "BgpPeerID", "TenantNetworkID" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork",
                column: "VpnID",
                principalTable: "Vpn",
                principalColumn: "VpnID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
