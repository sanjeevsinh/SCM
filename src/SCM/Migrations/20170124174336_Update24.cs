using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VpnVrf_Vpn_VpnID1",
                table: "VpnVrf");

            migrationBuilder.DropForeignKey(
                name: "FK_VpnVrf_Vrfs_VrfID1",
                table: "VpnVrf");

            migrationBuilder.DropIndex(
                name: "IX_VpnVrf_VpnID1",
                table: "VpnVrf");

            migrationBuilder.DropIndex(
                name: "IX_VpnVrf_VrfID1",
                table: "VpnVrf");

            migrationBuilder.DropColumn(
                name: "VpnID1",
                table: "VpnVrf");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "VpnVrf");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VpnID1",
                table: "VpnVrf",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "VpnVrf",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VpnID1",
                table: "VpnVrf",
                column: "VpnID1");

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VrfID1",
                table: "VpnVrf",
                column: "VrfID1");

            migrationBuilder.AddForeignKey(
                name: "FK_VpnVrf_Vpn_VpnID1",
                table: "VpnVrf",
                column: "VpnID1",
                principalTable: "Vpn",
                principalColumn: "VpnID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VpnVrf_Vrfs_VrfID1",
                table: "VpnVrf",
                column: "VrfID1",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
