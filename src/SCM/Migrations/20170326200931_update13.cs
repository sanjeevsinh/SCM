using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class update13 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiPortPort");

            migrationBuilder.DropColumn(
                name: "BgpPeerSourceIpAddress",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "MultiPort");

            migrationBuilder.AddColumn<string>(
                name: "LocalFailureDetectionIpAddress",
                table: "MultiPort",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RemoteFailureDetectionIpAddress",
                table: "MultiPort",
                maxLength: 15,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "TenantID",
                table: "MultiPort",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiPort",
                table: "Interface",
                nullable: false,
                defaultValue: false);

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
                name: "IX_MultiPort_TenantID",
                table: "MultiPort",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_MultiPortID",
                table: "MultiPortMember",
                column: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortMember_PortID",
                table: "MultiPortMember",
                column: "PortID");

            migrationBuilder.AddForeignKey(
                name: "FK_MultiPort_Tenant_TenantID",
                table: "MultiPort",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MultiPort_Tenant_TenantID",
                table: "MultiPort");

            migrationBuilder.DropTable(
                name: "MultiPortMember");

            migrationBuilder.DropIndex(
                name: "IX_MultiPort_TenantID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "LocalFailureDetectionIpAddress",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "RemoteFailureDetectionIpAddress",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "TenantID",
                table: "MultiPort");

            migrationBuilder.DropColumn(
                name: "IsMultiPort",
                table: "Interface");

            migrationBuilder.AddColumn<string>(
                name: "BgpPeerSourceIpAddress",
                table: "MultiPort",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "MultiPort",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "MultiPortPort",
                columns: table => new
                {
                    MultiPortPortID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MultiPortID = table.Column<int>(nullable: false),
                    PortID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPortPort", x => x.MultiPortPortID);
                    table.ForeignKey(
                        name: "FK_MultiPortPort_MultiPort_MultiPortID",
                        column: x => x.MultiPortID,
                        principalTable: "MultiPort",
                        principalColumn: "MultiPortID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MultiPortPort_Port_PortID",
                        column: x => x.PortID,
                        principalTable: "Port",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_MultiPortID",
                table: "MultiPortPort",
                column: "MultiPortID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortPort_PortID",
                table: "MultiPortPort",
                column: "PortID");
        }
    }
}
