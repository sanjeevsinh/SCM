using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleInterface_DeviceID_ID",
                table: "BundleInterface");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "BundleInterface");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "BundleInterface",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_DeviceID_Name",
                table: "BundleInterface",
                columns: new[] { "DeviceID", "Name" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BundleInterface_DeviceID_Name",
                table: "BundleInterface");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "BundleInterface");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "BundleInterface",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BundleInterface_DeviceID_ID",
                table: "BundleInterface",
                columns: new[] { "DeviceID", "ID" },
                unique: true);
        }
    }
}
