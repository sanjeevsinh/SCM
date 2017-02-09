using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnAttachmentSetID",
                table: "VpnTenantNetwork");

            migrationBuilder.AddColumn<string>(
                name: "BgpPeerSourceIpAddress",
                table: "MultiPort",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID",
                table: "VpnTenantNetwork",
                column: "TenantNetworkID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropColumn(
                name: "BgpPeerSourceIpAddress",
                table: "MultiPort");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_TenantNetworkID_VpnAttachmentSetID",
                table: "VpnTenantNetwork",
                columns: new[] { "TenantNetworkID", "VpnAttachmentSetID" },
                unique: true);
        }
    }
}
