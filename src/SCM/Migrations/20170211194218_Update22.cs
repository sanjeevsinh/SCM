using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update22 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "InterfaceVlan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
