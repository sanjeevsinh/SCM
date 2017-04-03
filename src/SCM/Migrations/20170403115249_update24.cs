using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class update24 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MultiPortVlan_MultiPortID",
                table: "MultiPortVlan",
                column: "MultiPortID");

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPortVlan_MultiPort_MultiPortID",
                table: "MultiPortVlan",
                column: "MultiPortID",
                principalTable: "MultiPort",
                principalColumn: "MultiPortID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultiPortVlan_MultiPort_MultiPortID",
                table: "MultiPortVlan");

            migrationBuilder.DropIndex(
                name: "IX_MultiPortVlan_MultiPortID",
                table: "MultiPortVlan");
        }
    }
}
