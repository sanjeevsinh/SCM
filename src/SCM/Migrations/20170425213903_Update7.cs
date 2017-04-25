using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Vrf_VrfID1",
                table: "Vif");

            migrationBuilder.DropIndex(
                name: "IX_Vif_VrfID1",
                table: "Vif");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "Vif");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "Vif",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "Vif",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "Vif",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "Vif",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "Vif",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vif_VrfID1",
                table: "Vif",
                column: "VrfID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Vrf_VrfID1",
                table: "Vif",
                column: "VrfID1",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
