using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_Region_RegionID",
                table: "RouteTarget");

            migrationBuilder.DropIndex(
                name: "IX_RouteTarget_RegionID",
                table: "RouteTarget");

            migrationBuilder.DropColumn(
                name: "RegionID",
                table: "RouteTarget");
        }
    }
}
