using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutonomousSystem",
                table: "Device");

            migrationBuilder.DropColumn(
                name: "ManagementIpAddress",
                table: "Device");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Plane",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Plane_Name",
                table: "Plane",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Plane_Name",
                table: "Plane");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Plane",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AutonomousSystem",
                table: "Device",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ManagementIpAddress",
                table: "Device",
                maxLength: 15,
                nullable: true);
        }
    }
}
