using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VlanTagRangeID",
                table: "InterfaceVlan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VlanTagRangeID1",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VlanTagRange",
                columns: table => new
                {
                    VlanTagRangeID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    VlanTagRangeCount = table.Column<int>(nullable: false),
                    VlanTagRangeStart = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VlanTagRange", x => x.VlanTagRangeID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID",
                table: "InterfaceVlan",
                column: "VlanTagRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID1",
                table: "InterfaceVlan",
                column: "VlanTagRangeID1");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID",
                table: "InterfaceVlan",
                column: "VlanTagRangeID",
                principalTable: "VlanTagRange",
                principalColumn: "VlanTagRangeID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID1",
                table: "InterfaceVlan",
                column: "VlanTagRangeID1",
                principalTable: "VlanTagRange",
                principalColumn: "VlanTagRangeID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID",
                table: "InterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_VlanTagRange_VlanTagRangeID1",
                table: "InterfaceVlan");

            migrationBuilder.DropTable(
                name: "VlanTagRange");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID",
                table: "InterfaceVlan");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_VlanTagRangeID1",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "VlanTagRangeID",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "VlanTagRangeID1",
                table: "InterfaceVlan");
        }
    }
}
