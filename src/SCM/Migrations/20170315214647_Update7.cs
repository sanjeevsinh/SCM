using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID1",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID1",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "VlanTagRangeID1",
                table: "InterfaceVlan");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VlanTagRangeID1",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID1",
                table: "InterfaceVlan",
                column: "VlanTagRangeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID1",
                table: "InterfaceVlan",
                column: "VlanTagRangeID1",
                principalTable: "VlanTagRange",
                principalColumn: "VlanTagRangeID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
