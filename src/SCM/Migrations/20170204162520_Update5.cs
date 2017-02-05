using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BgpPeer_VrfID",
                table: "BgpPeer");

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeer_VrfID_IpAddress",
                table: "BgpPeer",
                columns: new[] { "VrfID", "IpAddress" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BgpPeer_VrfID_IpAddress",
                table: "BgpPeer");

            migrationBuilder.CreateIndex(
                name: "IX_BgpPeer_VrfID",
                table: "BgpPeer",
                column: "VrfID");
        }
    }
}
