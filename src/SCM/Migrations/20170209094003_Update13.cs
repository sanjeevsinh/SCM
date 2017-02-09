using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Extranet",
                columns: table => new
                {
                    ExtranetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExtranetVpnID = table.Column<int>(nullable: false),
                    MemberVpnID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Extranet", x => x.ExtranetID);
                    table.ForeignKey(
                        name: "FK_Extranet_Vpn_ExtranetVpnID",
                        column: x => x.ExtranetVpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Extranet_Vpn_MemberVpnID",
                        column: x => x.MemberVpnID,
                        principalTable: "Vpn",
                        principalColumn: "VpnID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Extranet_MemberVpnID",
                table: "Extranet",
                column: "MemberVpnID");

            migrationBuilder.CreateIndex(
                name: "IX_Extranet_ExtranetVpnID_MemberVpnID",
                table: "Extranet",
                columns: new[] { "ExtranetVpnID", "MemberVpnID" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Extranet");
        }
    }
}
