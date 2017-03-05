using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID",
                table: "TenantCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID1",
                table: "TenantCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID1",
                table: "TenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_TenantID1",
                table: "TenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_TenantCommunity_TenantID1",
                table: "TenantCommunity");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "TenantNetwork");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "TenantCommunity");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID",
                table: "TenantCommunity",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID",
                table: "TenantCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork");

            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "TenantNetwork",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "TenantCommunity",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_TenantID1",
                table: "TenantNetwork",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_TenantID1",
                table: "TenantCommunity",
                column: "TenantID1");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID",
                table: "TenantCommunity",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantCommunity_Tenant_TenantID1",
                table: "TenantCommunity",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID",
                table: "TenantNetwork",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID1",
                table: "TenantNetwork",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
