using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "Port",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "BundleInterface",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Port_TenantID1",
                table: "Port",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_TenantID",
                table: "BundleInterface",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Tenant_TenantID",
                table: "BundleInterface",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Port_Tenant_TenantID1",
                table: "Port",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Tenant_TenantID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_Port_Tenant_TenantID1",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_TenantID1",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_BundleInterface_TenantID",
                table: "BundleInterface");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "BundleInterface");
        }
    }
}
