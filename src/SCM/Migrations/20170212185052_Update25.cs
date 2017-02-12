using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update25 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "BundleInterface",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_DeviceID1",
                table: "BundleInterface",
                column: "DeviceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Device_DeviceID1",
                table: "BundleInterface",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Device_DeviceID1",
                table: "BundleInterface");

            migrationBuilder.DropIndex(
                name: "IX_BundleInterface_DeviceID1",
                table: "BundleInterface");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "BundleInterface");
        }
    }
}
