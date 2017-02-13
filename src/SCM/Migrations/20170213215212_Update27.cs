using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BandwidthKbps",
                table: "PortBandwidth",
                newName: "BandwidthGbps");

            migrationBuilder.RenameIndex(
                name: "IX_PortBandwidth_BandwidthKbps",
                table: "PortBandwidth",
                newName: "IX_PortBandwidth_BandwidthGbps");

            migrationBuilder.RenameColumn(
                name: "BandwidthKbps",
                table: "InterfaceBandwidth",
                newName: "BandwidthGbps");

            migrationBuilder.RenameIndex(
                name: "IX_InterfaceBandwidth_BandwidthKbps",
                table: "InterfaceBandwidth",
                newName: "IX_InterfaceBandwidth_BandwidthGbps");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BandwidthGbps",
                table: "PortBandwidth",
                newName: "BandwidthKbps");

            migrationBuilder.RenameIndex(
                name: "IX_PortBandwidth_BandwidthGbps",
                table: "PortBandwidth",
                newName: "IX_PortBandwidth_BandwidthKbps");

            migrationBuilder.RenameColumn(
                name: "BandwidthGbps",
                table: "InterfaceBandwidth",
                newName: "BandwidthKbps");

            migrationBuilder.RenameIndex(
                name: "IX_InterfaceBandwidth_BandwidthGbps",
                table: "InterfaceBandwidth",
                newName: "IX_InterfaceBandwidth_BandwidthKbps");
        }
    }
}
