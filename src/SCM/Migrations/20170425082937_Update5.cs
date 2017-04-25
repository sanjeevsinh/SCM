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
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.DropIndex(
                name: "IX_Vif_ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID1",
                table: "Vif",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vif_ContractBandwidthPoolID1",
                table: "Vif",
                column: "ContractBandwidthPoolID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID1",
                table: "Vif",
                column: "ContractBandwidthPoolID1",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
