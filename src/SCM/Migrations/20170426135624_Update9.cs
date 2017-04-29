using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Interface_InterfaceID",
                table: "Vlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Interface_InterfaceID1",
                table: "Vlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Vif_VifID",
                table: "Vlan");

            migrationBuilder.DropIndex(
                name: "IX_Vlan_InterfaceID1",
                table: "Vlan");

            migrationBuilder.DropColumn(
                name: "InterfaceID1",
                table: "Vlan");

            migrationBuilder.AlterColumn<int>(
                name: "VifID",
                table: "Vlan",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Interface_InterfaceID",
                table: "Vlan",
                column: "InterfaceID",
                principalTable: "Interface",
                principalColumn: "InterfaceID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Vif_VifID",
                table: "Vlan",
                column: "VifID",
                principalTable: "Vif",
                principalColumn: "VifID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Interface_InterfaceID",
                table: "Vlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Vlan_Vif_VifID",
                table: "Vlan");

            migrationBuilder.AlterColumn<int>(
                name: "VifID",
                table: "Vlan",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InterfaceID1",
                table: "Vlan",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vlan_InterfaceID1",
                table: "Vlan",
                column: "InterfaceID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Interface_InterfaceID",
                table: "Vlan",
                column: "InterfaceID",
                principalTable: "Interface",
                principalColumn: "InterfaceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Interface_InterfaceID1",
                table: "Vlan",
                column: "InterfaceID1",
                principalTable: "Interface",
                principalColumn: "InterfaceID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vlan_Vif_VifID",
                table: "Vlan",
                column: "VifID",
                principalTable: "Vif",
                principalColumn: "VifID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
