using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class update16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "MultiPort",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeviceID",
                table: "MultiPort",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "MultiPort",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterfaceBandwidthID",
                table: "MultiPort",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VrfID",
                table: "MultiPort",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_ContractBandwidthPoolID",
                table: "MultiPort",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_DeviceID",
                table: "MultiPort",
                column: "DeviceID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_DeviceID1",
                table: "MultiPort",
                column: "DeviceID1");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_InterfaceBandwidthID",
                table: "MultiPort",
                column: "InterfaceBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_VrfID",
                table: "MultiPort",
                column: "VrfID");

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "MultiPort",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_Device_DeviceID",
                table: "MultiPort",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_Device_DeviceID1",
                table: "MultiPort",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_InterfaceBandwidth_InterfaceBandwidthID",
                table: "MultiPort",
                column: "InterfaceBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_Vrf_VrfID",
                table: "MultiPort",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "MultiPort");

            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Device_DeviceID",
                table: "MultiPort");

            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Device_DeviceID1",
                table: "MultiPort");

            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_InterfaceBandwidth_InterfaceBandwidthID",
                table: "MultiPort");

            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Vrf_VrfID",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_ContractBandwidthPoolID",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_DeviceID",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_DeviceID1",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_InterfaceBandwidthID",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_VrfID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "DeviceID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "InterfaceBandwidthID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "VrfID",
                table: "MultiPort");
        }
    }
}
