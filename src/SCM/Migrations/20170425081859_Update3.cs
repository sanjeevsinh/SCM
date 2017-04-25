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
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "Attachment",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "Attachment",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "Attachment",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Vrf_VrfID",
                table: "Attachment",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
