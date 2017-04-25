using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Device_DeviceID1",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_DeviceID1",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "Attachment");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Device_DeviceID",
                table: "Attachment",
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
        }
    }
}
