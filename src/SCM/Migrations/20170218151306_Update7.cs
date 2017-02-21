using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class Update7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VpnTenantCommunity",
                columns: table => new
                {
                    VpnTenantCommunityID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantCommunityID = table.Column<int>(nullable: false),
                    VpnAttachmentSetID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VpnTenantCommunity", x => x.VpnTenantCommunityID);
                    table.ForeignKey(
                        name: "FK_VpnTenantCommunity_TenantCommunity_TenantCommunityID",
                        column: x => x.TenantCommunityID,
                        principalTable: "TenantCommunity",
                        principalColumn: "TenantCommunityID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VpnTenantCommunity_VpnAttachmentSet_VpnAttachmentSetID",
                        column: x => x.VpnAttachmentSetID,
                        principalTable: "VpnAttachmentSet",
                        principalColumn: "VpnAttachmentSetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantCommunity_TenantCommunityID",
                table: "VpnTenantCommunity",
                column: "TenantCommunityID");

            migrationBuilder.CreateIndex(
                name: "IX_VpnTenantCommunity_VpnAttachmentSetID",
                table: "VpnTenantCommunity",
                column: "VpnAttachmentSetID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VpnTenantCommunity");
        }
    }
}
