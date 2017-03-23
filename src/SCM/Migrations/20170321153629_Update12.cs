using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.AlterColumn<int>(
                name: "RouteTargetRangeID",
                table: "RouteTarget",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID",
                principalTable: "RouteTargetRange",
                principalColumn: "RouteTargetRangeID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.AlterColumn<int>(
                name: "RouteTargetRangeID",
                table: "RouteTarget",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID",
                principalTable: "RouteTargetRange",
                principalColumn: "RouteTargetRangeID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
