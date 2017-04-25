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
                name: "FK_Vif_Tenant_TenantID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Tenant_TenantID1",
                table: "Vif");

            migrationBuilder.DropIndex(
                name: "IX_Vif_TenantID1",
                table: "Vif");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "Vif");

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Tenant_TenantID",
                table: "Vif",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Tenant_TenantID",
                table: "Vif");

            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "Vif",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vif_TenantID1",
                table: "Vif",
                column: "TenantID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Tenant_TenantID",
                table: "Vif",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Tenant_TenantID1",
                table: "Vif",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
