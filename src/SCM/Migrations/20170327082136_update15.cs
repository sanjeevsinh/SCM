using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class update15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiPortMember");

            migrationBuilder.RenameColumn(
                name: "MultiPortMemberID",
                table: "Port",
                newName: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_Port_MultiPortID",
                table: "Port",
                column: "MultiPortID");

            migrationBuilder.AddForeignKey(
                name: "FK_Port_MultiPort_MultiPortID",
                table: "Port",
                column: "MultiPortID",
                principalTable: "MultiPort",
                principalColumn: "MultiPortID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Port_MultiPort_MultiPortID",
                table: "Port");

            migrationBuilder.DropIndex(
                name: "IX_Port_MultiPortID",
                table: "Port");

            migrationBuilder.RenameColumn(
                name: "MultiPortID",
                table: "Port",
                newName: "MultiPortMemberID");

            migrationBuilder.CreateTable(
                name: "MultiPortMember",
                columns: table => new
                {
                    MultiPortMemberID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MultiPortID = table.Column<int>(nullable: false),
                    PortID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPortMember", x => x.MultiPortMemberID);
                    table.ForeignKey(
                        name: "FK_MultiPortMember_MultiPort_MultiPortID",
                        column: x => x.MultiPortID,
                        principalTable: "MultiPort",
                        principalColumn: "MultiPortID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiPortMember_Port_PortID",
                        column: x => x.PortID,
                        principalTable: "Port",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_MultiPortID",
                table: "MultiPortMember",
                column: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember",
                column: "PortID",
                unique: true);
        }
    }
}
