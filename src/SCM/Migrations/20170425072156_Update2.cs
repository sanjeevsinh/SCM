using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment");

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

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "Attachment",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_DeviceID1",
                table: "Attachment",
                column: "DeviceID1");

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
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID1",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_DeviceID1",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "Attachment");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "Vrf",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_DeviceID1",
                table: "Vrf",
                column: "DeviceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
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
    }
}
