using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
