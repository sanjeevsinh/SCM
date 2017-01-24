using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VpnVrf",
                table: "VpnVrf");

            migrationBuilder.DropIndex(
                name: "IX_VpnVrf_VpnID_VrfID",
                table: "VpnVrf");

            migrationBuilder.DropColumn(
                name: "VpnVrfID",
                table: "VpnVrf");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "VpnVrf",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "VpnID",
                table: "VpnVrf",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VpnVrf",
                table: "VpnVrf",
                columns: new[] { "VpnID", "VrfID" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VpnVrf",
                table: "VpnVrf");

            migrationBuilder.AlterColumn<int>(
                name: "VrfID",
                table: "VpnVrf",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "VpnID",
                table: "VpnVrf",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "VpnVrfID",
                table: "VpnVrf",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VpnVrf",
                table: "VpnVrf",
                column: "VpnVrfID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnVrf_VpnID_VrfID",
                table: "VpnVrf",
                columns: new[] { "VpnID", "VrfID" },
                unique: true);
        }
    }
}
