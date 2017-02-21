using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TenantID1",
                table: "TenantNetwork",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TenantCommunity",
                columns: table => new
                {
                    TenantCommunityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AutonomousSystemNumber = table.Column<int>(nullable: false),
                    Number = table.Column<int>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: true),
                    TenantID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantCommunity", x => x.TenantCommunityID);
                    table.ForeignKey(
                        name: "FK_TenantCommunity_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantCommunity_Tenant_TenantID1",
                        column: x => x.TenantID1,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_TenantID1",
                table: "TenantNetwork",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_TenantID",
                table: "TenantCommunity",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_TenantID1",
                table: "TenantCommunity",
                column: "TenantID1");

            migrationBuilder.CreateIndex(
                name: "IX_TenantCommunity_AutonomousSystemNumber_Number",
                table: "TenantCommunity",
                columns: new[] { "AutonomousSystemNumber", "Number" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID1",
                table: "TenantNetwork",
                column: "TenantID1",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_Tenant_TenantID1",
                table: "TenantNetwork");

            migrationBuilder.DropTable(
                name: "TenantCommunity");

            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_TenantID1",
                table: "TenantNetwork");

            migrationBuilder.DropColumn(
                name: "TenantID1",
                table: "TenantNetwork");
        }
    }
}
