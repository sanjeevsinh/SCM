using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update21 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "InterfaceVlan",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort",
                column: "PortID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "InterfaceVlan",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort",
                column: "PortID");
        }
    }
}
