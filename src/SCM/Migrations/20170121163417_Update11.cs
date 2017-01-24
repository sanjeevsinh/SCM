using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PhysicalPortBandwidthID",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_PhysicalPortBandwidthID",
                table: "Port");

            migrationBuilder.RenameColumn(
                name: "PhysicalPortBandwidthID",
                table: "PhysicalPortBandwidth",
                newName: "PortBandwidthID");

            migrationBuilder.RenameColumn(
                name: "LogicalBandwidthID",
                table: "LogicalBandwidth",
                newName: "InterfaceBandwidthID");

            migrationBuilder.AddColumn<int>(
                name: "PortBandwidthID",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_PortBandwidthID",
                table: "Port",
                column: "PortBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PortBandwidthID",
                table: "Port",
                column: "PortBandwidthID",
                principalTable: "PhysicalPortBandwidth",
                principalColumn: "PortBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PortBandwidthID",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_PortBandwidthID",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "PortBandwidthID",
                table: "Port");

            migrationBuilder.RenameColumn(
                name: "PortBandwidthID",
                table: "PhysicalPortBandwidth",
                newName: "PhysicalPortBandwidthID");

            migrationBuilder.RenameColumn(
                name: "InterfaceBandwidthID",
                table: "LogicalBandwidth",
                newName: "LogicalBandwidthID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_PhysicalPortBandwidthID",
                table: "Port",
                column: "PhysicalPortBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_PhysicalPortBandwidth_PhysicalPortBandwidthID",
                table: "Port",
                column: "PhysicalPortBandwidthID",
                principalTable: "PhysicalPortBandwidth",
                principalColumn: "PhysicalPortBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
