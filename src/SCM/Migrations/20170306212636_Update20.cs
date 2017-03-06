using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_Tenant_TenantID1",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_TenantID1",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "Port");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_TenantID1",
                table: "Port",
                column: "TenantID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_Tenant_TenantID1",
                table: "Port",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
