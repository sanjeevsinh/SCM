using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PortBandwidthID",
                table: "Port");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PhysicalPortBandwidth",
                table: "PhysicalPortBandwidth");

            migrationBuilder.RenameTable(
                name: "PhysicalPortBandwidth",
                newName: "PortBandwidth");

            migrationBuilder.RenameIndex(
                name: "IX_PhysicalPortBandwidth_BandwidthKbps",
                table: "PortBandwidth",
                newName: "IX_PortBandwidth_BandwidthKbps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PortBandwidth",
                table: "PortBandwidth",
                column: "PortBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_PortBandwidth_PortBandwidthID",
                table: "Port",
                column: "PortBandwidthID",
                principalTable: "PortBandwidth",
                principalColumn: "PortBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_PortBandwidth_PortBandwidthID",
                table: "Port");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PortBandwidth",
                table: "PortBandwidth");

            migrationBuilder.RenameTable(
                name: "PortBandwidth",
                newName: "PhysicalPortBandwidth");

            migrationBuilder.RenameIndex(
                name: "IX_PortBandwidth_BandwidthKbps",
                table: "PhysicalPortBandwidth",
                newName: "IX_PhysicalPortBandwidth_BandwidthKbps");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PhysicalPortBandwidth",
                table: "PhysicalPortBandwidth",
                column: "PortBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PortBandwidthID",
                table: "Port",
                column: "PortBandwidthID",
                principalTable: "PhysicalPortBandwidth",
                principalColumn: "PortBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
