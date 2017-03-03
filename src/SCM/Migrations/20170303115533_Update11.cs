using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractBandwidthPoolID",
                table: "InterfaceVlan",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContractBandwidthPool",
                columns: table => new
                {
                    ContractBandwidthPoolID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractBandwidthID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContractBandwidthPool", x => x.ContractBandwidthPoolID);
                    table.ForeignKey(
                        name: "FK_ContractBandwidthPool_ContractBandwidth_ContractBandwidthID",
                        column: x => x.ContractBandwidthID,
                        principalTable: "ContractBandwidth",
                        principalColumn: "ContractBandwidthID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterfaceVlan_ContractBandwidthPoolID",
                table: "InterfaceVlan",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_ContractBandwidthPool_ContractBandwidthID",
                table: "ContractBandwidthPool",
                column: "ContractBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "InterfaceVlan",
                column: "ContractBandwidthPoolID",
                principalTable: "ContractBandwidthPool",
                principalColumn: "ContractBandwidthPoolID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_ContractBandwidthPool_ContractBandwidthPoolID",
                table: "InterfaceVlan");

            migrationBuilder.DropTable(
                name: "ContractBandwidthPool");

            migrationBuilder.DropIndex(
                name: "IX_InterfaceVlan_ContractBandwidthPoolID",
                table: "InterfaceVlan");

            migrationBuilder.DropColumn(
                name: "ContractBandwidthPoolID",
                table: "InterfaceVlan");
        }
    }
}
