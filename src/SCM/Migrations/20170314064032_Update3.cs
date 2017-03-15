using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RouteDistinguisherRangeID",
                table: "Vrf",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouteDistinguisherRangeID1",
                table: "Vrf",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RouteTargetRangeID",
                table: "RouteTarget",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RouteTargetRangeID1",
                table: "RouteTarget",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RouteDistinguisherRange",
                columns: table => new
                {
                    RouteDistinguisherRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldCount = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldStart = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteDistinguisherRange", x => x.RouteDistinguisherRangeID);
                });

            migrationBuilder.CreateTable(
                name: "RouteTargetRange",
                columns: table => new
                {
                    RouteTargetRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AdministratorSubField = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldCount = table.Column<int>(nullable: false),
                    AssignedNumberSubFieldStart = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteTargetRange", x => x.RouteTargetRangeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID",
                table: "Vrf",
                column: "RouteDistinguisherRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID1",
                table: "Vrf",
                column: "RouteDistinguisherRangeID1");

            migrationBuilder.CreateIndex(
                name: "IX_RouteTarget_RouteTargetRangeID",
                table: "RouteTarget",
                column: "RouteTargetRangeID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropTable(
                name: "RouteDistinguisherRange");

            migrationBuilder.DropTable(
                name: "RouteTargetRange");

            migrationBuilder.DropIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_Vrf_RouteDistinguisherRangeID1",
                table: "Vrf");

            migrationBuilder.DropIndex(
                name: "IX_RouteTarget_RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.DropIndex(
                name: "IX_RouteTarget_RouteTargetRangeID1",
                table: "RouteTarget");

            migrationBuilder.DropColumn(
                name: "RouteDistinguisherRangeID",
                table: "Vrf");

            migrationBuilder.DropColumn(
                name: "RouteDistinguisherRangeID1",
                table: "Vrf");

            migrationBuilder.DropColumn(
                name: "RouteTargetRangeID",
                table: "RouteTarget");

            migrationBuilder.DropColumn(
                name: "RouteTargetRangeID1",
                table: "RouteTarget");
        }
    }
}
