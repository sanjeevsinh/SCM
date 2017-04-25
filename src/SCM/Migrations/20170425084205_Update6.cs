using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_Interface_InterfaceID1",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_InterfaceID1",
                table: "Port");

            migrationBuilder.DropColumn(
                name: "InterfaceID1",
                table: "Port");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterfaceID1",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Port_InterfaceID1",
                table: "Port",
                column: "InterfaceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_Interface_InterfaceID1",
                table: "Port",
                column: "InterfaceID1",
                principalTable: "Interface",
                principalColumn: "InterfaceID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
