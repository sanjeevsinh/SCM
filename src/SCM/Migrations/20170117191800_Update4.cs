using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Tenant_TenantID",
                table: "Vlan");

            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "Vrfs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "TenantID",
                table: "Vlan",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Vrfs_TenantID",
                table: "Vrfs",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Tenant_TenantID",
                table: "Vlan",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrfs_Tenant_TenantID",
                table: "Vrfs",
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

            migrationBuilder.DropForeignKey(
                name: "FK_Vrfs_Tenant_TenantID",
                table: "Vrfs");

            migrationBuilder.DropIndex(
                name: "IX_Vrfs_TenantID",
                table: "Vrfs");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "Vrfs");

            migrationBuilder.AlterColumn<int>(
                name: "TenantID",
                table: "Vlan",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Tenant_TenantID",
                table: "Vlan",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
