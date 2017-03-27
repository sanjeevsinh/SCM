using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class update14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember");

            migrationBuilder.AddColumn<int>(
                name: "MultiPortMemberID",
                table: "Port",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember",
                column: "PortID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember");

            migrationBuilder.DropColumn(
                name: "MultiPortMemberID",
                table: "Port");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember",
                column: "PortID");
        }
    }
}
