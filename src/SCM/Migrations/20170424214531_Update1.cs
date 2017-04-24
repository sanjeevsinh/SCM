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
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID1",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_Device_DeviceID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_Device_DeviceID1",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Vrf_VrfID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Vrf_VrfID1",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Vif_ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.DropIndex(
                name: "IX_Vif_VrfID1",
                table: "Vif");

            migrationBuilder.DropIndex(
                name: "IX_Interface_DeviceID1",
                table: "Interface");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_ContractBandwidthPoolID1",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID1",
                table: "Vif");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "Vif");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "Interface");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID1",
                table: "Attachment");

            migrationBuilder.RenameColumn(
                name: "VrfID1",
                table: "Attachment",
                newName: "DeviceID1");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_VrfID1",
                table: "Attachment",
                newName: "IX_Attachment_DeviceID1");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "Vrf",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_DeviceID1",
                table: "Vrf",
                column: "DeviceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Device_DeviceID1",
                table: "Attachment",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_Device_DeviceID",
                table: "Interface",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Vrf_VrfID",
                table: "Vif",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID1",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_Device_DeviceID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vif_Vrf_VrfID",
                table: "Vif");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Vrf_DeviceID1",
                table: "Vrf");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "Vrf");

            migrationBuilder.RenameColumn(
                name: "DeviceID1",
                table: "Attachment",
                newName: "VrfID1");

            migrationBuilder.RenameIndex(
                name: "IX_Attachment_DeviceID1",
                table: "Attachment",
                newName: "IX_Attachment_VrfID1");

            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID1",
                table: "Vif",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "Vif",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "Interface",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID1",
                table: "Attachment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vif_ContractBandwidthPoolID1",
                table: "Vif",
                column: "ContractBandwidthPoolID1");

            migrationBuilder.CreateIndex(
                name: "IX_Vif_VrfID1",
                table: "Vif",
                column: "VrfID1");

            migrationBuilder.CreateIndex(
                name: "IX_Interface_DeviceID1",
                table: "Interface",
                column: "DeviceID1");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_ContractBandwidthPoolID1",
                table: "Attachment",
                column: "ContractBandwidthPoolID1");

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
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_Interface_Device_DeviceID",
                table: "Interface",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_Device_DeviceID1",
                table: "Interface",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Vrf_VrfID",
                table: "Vif",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vif_Vrf_VrfID1",
                table: "Vif",
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
