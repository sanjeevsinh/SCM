using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_Region_RegionID",
                table: "RouteTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_Vpn_Plane_PlaneID",
                table: "Vpn");

            migrationBuilder.DropIndex(
                name: "IX_RouteTarget_RegionID",
                table: "RouteTarget");

            migrationBuilder.DropColumn(
                name: "RegionID",
                table: "RouteTarget");

            migrationBuilder.AlterColumn<int>(
                name: "PlaneID",
                table: "Vpn",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "RegionID",
                table: "Vpn",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vpn_RegionID",
                table: "Vpn",
                column: "RegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vpn_Plane_PlaneID",
                table: "Vpn",
                column: "PlaneID",
                principalTable: "Plane",
                principalColumn: "PlaneID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vpn_Region_RegionID",
                table: "Vpn",
                column: "RegionID",
                principalTable: "Region",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vpn_Plane_PlaneID",
                table: "Vpn");

            migrationBuilder.DropForeignKey(
                name: "FK_Vpn_Region_RegionID",
                table: "Vpn");

            migrationBuilder.DropIndex(
                name: "IX_Vpn_RegionID",
                table: "Vpn");

            migrationBuilder.DropColumn(
                name: "RegionID",
                table: "Vpn");

            migrationBuilder.AlterColumn<int>(
                name: "PlaneID",
                table: "Vpn",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegionID",
                table: "RouteTarget",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_RegionID",
                table: "RouteTarget",
                column: "RegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_Region_RegionID",
                table: "RouteTarget",
                column: "RegionID",
                principalTable: "Region",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vpn_Plane_PlaneID",
                table: "Vpn",
                column: "PlaneID",
                principalTable: "Plane",
                principalColumn: "PlaneID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
