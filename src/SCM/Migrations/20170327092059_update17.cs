using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class update17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Device_DeviceID",
                table: "MultiPort");

            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Device_DeviceID1",
                table: "MultiPort");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_DeviceID1",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "MultiPort");

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_Device_DeviceID",
                table: "MultiPort",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Device_DeviceID",
                table: "MultiPort");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "MultiPort",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MultiPort_DeviceID1",
                table: "MultiPort",
                column: "DeviceID1");

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
        }
    }
}
