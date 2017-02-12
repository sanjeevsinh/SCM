using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Device_DeviceID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Device_DeviceID1",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfacePort_Port_PortID",
                table: "BundleInterfacePort");

            migrationBuilder.DropIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort");

            migrationBuilder.DropIndex(
                name: "IX_BundleInterface_DeviceID1",
                table: "BundleInterface");

            migrationBuilder.DropColumn(
                name: "DeviceID1",
                table: "BundleInterface");

            migrationBuilder.AddColumn<int>(
                name: "BundleInterfacePortID",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_BundleInterfacePortID",
                table: "Port",
                column: "BundleInterfacePortID");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort",
                column: "PortID");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Device_DeviceID",
                table: "BundleInterface",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfacePort_Port_PortID",
                table: "BundleInterfacePort",
                column: "PortID",
                principalTable: "Port",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Port_BundleInterfacePort_BundleInterfacePortID",
                table: "Port",
                column: "BundleInterfacePortID",
                principalTable: "BundleInterfacePort",
                principalColumn: "BundleInterfacePortID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Device_DeviceID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfacePort_Port_PortID",
                table: "BundleInterfacePort");

            migrationBuilder.DropForeignKey(
                name: "FK_Port_BundleInterfacePort_BundleInterfacePortID",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_BundleInterfacePortID",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort");

            migrationBuilder.DropColumn(
                name: "BundleInterfacePortID",
                table: "Port");

            migrationBuilder.AddColumn<int>(
                name: "DeviceID1",
                table: "BundleInterface",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterfacePort_PortID",
                table: "BundleInterfacePort",
                column: "PortID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_DeviceID1",
                table: "BundleInterface",
                column: "DeviceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Device_DeviceID",
                table: "BundleInterface",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Device_DeviceID1",
                table: "BundleInterface",
                column: "DeviceID1",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfacePort_Port_PortID",
                table: "BundleInterfacePort",
                column: "PortID",
                principalTable: "Port",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
