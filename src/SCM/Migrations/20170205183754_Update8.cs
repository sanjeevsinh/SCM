using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_VpnTenantNetwork_VpnID",
                table: "VpnTenantNetwork");

            migrationBuilder.DropColumn(
                name: "VpnID",
                table: "VpnTenantNetwork");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VpnID",
                table: "VpnTenantNetwork",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantNetwork_VpnID",
                table: "VpnTenantNetwork",
                column: "VpnID");

            migrationBuilder.AddForeignKey(
                name: "FK_VpnTenantNetwork_Vpn_VpnID",
                table: "VpnTenantNetwork",
                column: "VpnID",
                principalTable: "Vpn",
                principalColumn: "VpnID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
