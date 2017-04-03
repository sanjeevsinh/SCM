using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SCM.Migrations
{
    public partial class update23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MultiPortVlan",
                columns: table => new
                {
                    MultiPortVlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ContractBandwidthPoolID = table.Column<int>(nullable: true),
                    IsLayer3 = table.Column<bool>(nullable: false),
                    MultiPortID = table.Column<int>(nullable: false),
                    RequiresSync = table.Column<bool>(nullable: false),
                    RowVersion = table.Column<byte[]>(rowVersion: true, nullable: true),
                    TenantID = table.Column<int>(nullable: false),
                    VlanTag = table.Column<int>(nullable: false),
                    VlanTagRangeID = table.Column<int>(nullable: true),
                    VrfID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MultiPortVlan", x => x.MultiPortVlanID);
                    table.ForeignKey(
                        name: "FK_MultiPortVlan_ContractBandwidthPool_ContractBandwidthPoolID",
                        column: x => x.ContractBandwidthPoolID,
                        principalTable: "ContractBandwidthPool",
                        principalColumn: "ContractBandwidthPoolID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MultiPortVlan_Tenant_TenantID",
                        column: x => x.TenantID,
                        principalTable: "Tenant",
                        principalColumn: "TenantID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MultiPortVlan_VlanTagRange_VlanTagRangeID",
                        column: x => x.VlanTagRangeID,
                        principalTable: "VlanTagRange",
                        principalColumn: "VlanTagRangeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MultiPortVlan_Vrf_VrfID",
                        column: x => x.VrfID,
                        principalTable: "Vrf",
                        principalColumn: "VrfID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortVlan_ContractBandwidthPoolID",
                table: "MultiPortVlan",
                column: "ContractBandwidthPoolID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortVlan_TenantID",
                table: "MultiPortVlan",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortVlan_VlanTagRangeID",
                table: "MultiPortVlan",
                column: "VlanTagRangeID");

            migrationBuilder.CreateIndex(
                name: "IX_MultiPortVlan_VrfID",
                table: "MultiPortVlan",
                column: "VrfID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MultiPortVlan");
        }
    }
}
