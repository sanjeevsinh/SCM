using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan");

            migrationBuilder.AddColumn<int>(
                name: "VrfID1",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "BundleInterfaceVlan",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "BundleInterfaceVlan",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
