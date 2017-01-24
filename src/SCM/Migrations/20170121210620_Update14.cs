using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Location_AlternateLocationID",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_AlternateLocationID",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "AlternateLocationID",
                table: "Location");

            migrationBuilder.AddColumn<int>(
                name: "AlternateLocationLocationID",
                table: "Location",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Location_AlternateLocationLocationID",
                table: "Location",
                column: "AlternateLocationLocationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Location_AlternateLocationLocationID",
                table: "Location",
                column: "AlternateLocationLocationID",
                principalTable: "Location",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Location_Location_AlternateLocationLocationID",
                table: "Location");

            migrationBuilder.DropIndex(
                name: "IX_Location_AlternateLocationLocationID",
                table: "Location");

            migrationBuilder.DropColumn(
                name: "AlternateLocationLocationID",
                table: "Location");

            migrationBuilder.AddColumn<int>(
                name: "AlternateLocationID",
                table: "Location",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Location_AlternateLocationID",
                table: "Location",
                column: "AlternateLocationID");

            migrationBuilder.AddForeignKey(
                name: "FK_Location_Location_AlternateLocationID",
                table: "Location",
                column: "AlternateLocationID",
                principalTable: "Location",
                principalColumn: "LocationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
