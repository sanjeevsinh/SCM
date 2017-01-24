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
                name: "InterfaceID",
                table: "Port",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PortID",
                table: "Interface",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterfaceID",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "PortID",
                table: "Interface");
        }
    }
}
