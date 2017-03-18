using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID1",
                table: "RouteTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID",
                table: "Vrf");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_RouteTarget_RouteTargetRangeID1",
                table: "RouteTarget");

            migrationBuilder.DropColumn(
                name: "RouteDistinguisherRangeID1",
                table: "Vrf");

            migrationBuilder.DropColumn(
                name: "RouteTargetRangeID1",
                table: "RouteTarget");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID",
                principalTable: "RouteTargetRange",
                principalColumn: "RouteTargetRangeID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID",
                table: "Vrf",
                column: "RouteDistinguisherRangeID",
                principalTable: "RouteDistinguisherRange",
                principalColumn: "RouteDistinguisherRangeID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID",
                table: "Vrf");

            migrationBuilder.AddColumn<int>(
                name: "RouteDistinguisherRangeID1",
                table: "Vrf",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteTargetRangeID1",
                table: "RouteTarget",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID1",
                table: "Vrf",
                column: "RouteDistinguisherRangeID1");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_RouteTargetRangeID1",
                table: "RouteTarget",
                column: "RouteTargetRangeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID",
                principalTable: "RouteTargetRange",
                principalColumn: "RouteTargetRangeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteTarget_RouteTargetRange_RouteTargetRangeID1",
                table: "RouteTarget",
                column: "RouteTargetRangeID1",
                principalTable: "RouteTargetRange",
                principalColumn: "RouteTargetRangeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID",
                table: "Vrf",
                column: "RouteDistinguisherRangeID",
                principalTable: "RouteDistinguisherRange",
                principalColumn: "RouteDistinguisherRangeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_RouteDistinguisherRange_RouteDistinguisherRangeID1",
                table: "Vrf",
                column: "RouteDistinguisherRangeID1",
                principalTable: "RouteDistinguisherRange",
                principalColumn: "RouteDistinguisherRangeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
