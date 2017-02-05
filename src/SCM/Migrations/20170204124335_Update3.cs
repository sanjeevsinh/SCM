using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "BgpPeer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "BgpPeer",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
