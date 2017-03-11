using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "InterfaceVlan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_TenantID",
                table: "InterfaceVlan",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Tenant_TenantID",
                table: "InterfaceVlan",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Tenant_TenantID",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_TenantID",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "InterfaceVlan");
        }
    }
}
