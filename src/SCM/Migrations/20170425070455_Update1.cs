using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID1",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID1",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_ContractBandwidthPoolID1",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_VrfID1",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID1",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "Attachment");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "Vrf",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterfaceID1",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_DeviceID1",
                table: "Vrf",
                column: "DeviceID1");

            migrationBuilder.CreateIndex(
                name: "IX_Port_InterfaceID1",
                table: "Port",
                column: "InterfaceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Port_Interface_InterfaceID1",
                table: "Port",
                column: "InterfaceID1",
                principalTable: "Interface",
                principalColumn: "InterfaceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_Device_DeviceID1",
                table: "Vrf",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Port_Interface_InterfaceID1",
                table: "Port");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Vrf_DeviceID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Port_InterfaceID1",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "Vrf");

            migrationBuilder.DropColumn(
                name: "InterfaceID1",
                table: "Port");

            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID1",
                table: "Attachment",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "Attachment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_ContractBandwidthPoolID1",
                table: "Attachment",
                column: "ContractBandwidthPoolID1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_VrfID1",
                table: "Attachment",
                column: "VrfID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID1",
                table: "Attachment",
                column: "ContractBandwidthPoolID1",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID1",
                table: "Attachment",
                column: "VrfID1",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
