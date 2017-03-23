using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresSync",
                table: "Vpn",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresSync",
                table: "InterfaceVlan",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresSync",
                table: "Interface",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresSync",
                table: "Vpn");

            migrationBuilder.DropColumn(
                name: "RequiresSync",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "RequiresSync",
                table: "Interface");
        }
    }
}
