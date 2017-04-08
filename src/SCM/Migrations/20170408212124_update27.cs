using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class update27 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MultiPortVlanID",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_MultiPortVlanID",
                table: "InterfaceVlan",
                column: "MultiPortVlanID");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_MultiPortVlan_MultiPortVlanID",
                table: "InterfaceVlan",
                column: "MultiPortVlanID",
                principalTable: "MultiPortVlan",
                principalColumn: "MultiPortVlanID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_MultiPortVlan_MultiPortVlanID",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_MultiPortVlanID",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "MultiPortVlanID",
                table: "InterfaceVlan");
        }
    }
}
