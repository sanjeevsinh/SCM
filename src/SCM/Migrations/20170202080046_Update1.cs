using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegionID",
                table: "AttachmentSet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_RegionID",
                table: "AttachmentSet",
                column: "RegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSet_Region_RegionID",
                table: "AttachmentSet",
                column: "RegionID",
                principalTable: "Region",
                principalColumn: "RegionID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentSet_Region_RegionID",
                table: "AttachmentSet");

            migrationBuilder.DropIndex(
                name: "IX_AttachmentSet_RegionID",
                table: "AttachmentSet");

            migrationBuilder.DropColumn(
                name: "RegionID",
                table: "AttachmentSet");
        }
    }
}
