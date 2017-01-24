using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "Vlan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vlan_TenantID",
                table: "Vlan",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Tenant_TenantID",
                table: "Vlan",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Tenant_TenantID",
                table: "Vlan");

            migrationBuilder.DropIndex(
                name: "IX_Vlan_TenantID",
                table: "Vlan");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "Vlan");
        }
    }
}
