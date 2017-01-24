using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "Vlan",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }
    }
}
