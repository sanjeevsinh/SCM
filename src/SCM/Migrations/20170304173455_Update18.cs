using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentSet_ContractBandwidth_ContractBandwidthID",
                table: "AttachmentSet");

            migrationBuilder.DropIndex(
                name: "IX_AttachmentSet_ContractBandwidthID",
                table: "AttachmentSet");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthID",
                table: "AttachmentSet");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthID",
                table: "AttachmentSet",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentSet_ContractBandwidthID",
                table: "AttachmentSet",
                column: "ContractBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSet_ContractBandwidth_ContractBandwidthID",
                table: "AttachmentSet",
                column: "ContractBandwidthID",
                principalTable: "ContractBandwidth",
                principalColumn: "ContractBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
