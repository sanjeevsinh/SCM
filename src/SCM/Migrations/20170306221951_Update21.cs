using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BundleOrMultiPortMemberBandwidthGbps",
                table: "InterfaceBandwidth",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SupportedByBundle",
                table: "InterfaceBandwidth",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SupportedByMultiPort",
                table: "InterfaceBandwidth",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BundleOrMultiPortMemberBandwidthGbps",
                table: "InterfaceBandwidth");

            migrationBuilder.DropColumn(
                name: "SupportedByBundle",
                table: "InterfaceBandwidth");

            migrationBuilder.DropColumn(
                name: "SupportedByMultiPort",
                table: "InterfaceBandwidth");
        }
    }
}
