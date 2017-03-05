using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                table: "ContractBandwidthPool");

            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "Interface",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "ContractBandwidthPool",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Interface_ContractBandwidthPoolID",
                table: "Interface",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidthPool_TenantID",
                table: "ContractBandwidthPool",
                column: "TenantID");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                table: "ContractBandwidthPool",
                column: "ContractBandwidthID",
                principalTable: "ContractBandwidth",
                principalColumn: "ContractBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ContractBandwidthPool_Tenant_TenantID",
                table: "ContractBandwidthPool",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Interface",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                table: "ContractBandwidthPool");

            migrationBuilder.DropForeignKey(
                name: "FK_ContractBandwidthPool_Tenant_TenantID",
                table: "ContractBandwidthPool");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Interface");

            migrationBuilder.DropIndex(
                name: "IX_Interface_ContractBandwidthPoolID",
                table: "Interface");

            migrationBuilder.DropIndex(
                name: "IX_ContractBandwidthPool_TenantID",
                table: "ContractBandwidthPool");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID",
                table: "Interface");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "ContractBandwidthPool");

            migrationBuilder.AddForeignKey(
                name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                table: "ContractBandwidthPool",
                column: "ContractBandwidthID",
                principalTable: "ContractBandwidth",
                principalColumn: "ContractBandwidthID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
