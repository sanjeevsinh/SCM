using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_InterfaceBandwidth_LogicalBandwidthID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_InterfaceBandwidth_LogicalBandwidthID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_InterfaceBandwidth_LogicalBandwidthID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_InterfaceBandwidth_LogicalBandwidthID",
                table: "InterfaceVlan");

            migrationBuilder.RenameColumn(
                name: "LogicalBandwidthID",
                table: "InterfaceVlan",
                newName: "InterfaceBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_InterfaceVlan_LogicalBandwidthID",
                table: "InterfaceVlan",
                newName: "IX_InterfaceVlan_InterfaceBandwidthID");

            migrationBuilder.RenameColumn(
                name: "LogicalBandwidthID",
                table: "Interface",
                newName: "InterfaceBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_Interface_LogicalBandwidthID",
                table: "Interface",
                newName: "IX_Interface_InterfaceBandwidthID");

            migrationBuilder.RenameColumn(
                name: "LogicalBandwidthID",
                table: "BundleInterfaceVlan",
                newName: "InterfaceBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_BundleInterfaceVlan_LogicalBandwidthID",
                table: "BundleInterfaceVlan",
                newName: "IX_BundleInterfaceVlan_InterfaceBandwidthID");

            migrationBuilder.RenameColumn(
                name: "LogicalBandwidthID",
                table: "BundleInterface",
                newName: "InterfaceBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_BundleInterface_LogicalBandwidthID",
                table: "BundleInterface",
                newName: "IX_BundleInterface_InterfaceBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_InterfaceBandwidth_InterfaceBandwidthID",
                table: "BundleInterface",
                column: "InterfaceBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_InterfaceBandwidth_InterfaceBandwidthID",
                table: "BundleInterfaceVlan",
                column: "InterfaceBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_InterfaceBandwidth_InterfaceBandwidthID",
                table: "Interface",
                column: "InterfaceBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_InterfaceBandwidth_InterfaceBandwidthID",
                table: "InterfaceVlan",
                column: "InterfaceBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_InterfaceBandwidth_InterfaceBandwidthID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_InterfaceBandwidth_InterfaceBandwidthID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_InterfaceBandwidth_InterfaceBandwidthID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_InterfaceBandwidth_InterfaceBandwidthID",
                table: "InterfaceVlan");

            migrationBuilder.RenameColumn(
                name: "InterfaceBandwidthID",
                table: "InterfaceVlan",
                newName: "LogicalBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_InterfaceVlan_InterfaceBandwidthID",
                table: "InterfaceVlan",
                newName: "IX_InterfaceVlan_LogicalBandwidthID");

            migrationBuilder.RenameColumn(
                name: "InterfaceBandwidthID",
                table: "Interface",
                newName: "LogicalBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_Interface_InterfaceBandwidthID",
                table: "Interface",
                newName: "IX_Interface_LogicalBandwidthID");

            migrationBuilder.RenameColumn(
                name: "InterfaceBandwidthID",
                table: "BundleInterfaceVlan",
                newName: "LogicalBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_BundleInterfaceVlan_InterfaceBandwidthID",
                table: "BundleInterfaceVlan",
                newName: "IX_BundleInterfaceVlan_LogicalBandwidthID");

            migrationBuilder.RenameColumn(
                name: "InterfaceBandwidthID",
                table: "BundleInterface",
                newName: "LogicalBandwidthID");

            migrationBuilder.RenameIndex(
                name: "IX_BundleInterface_InterfaceBandwidthID",
                table: "BundleInterface",
                newName: "IX_BundleInterface_LogicalBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_InterfaceBandwidth_LogicalBandwidthID",
                table: "BundleInterface",
                column: "LogicalBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_InterfaceBandwidth_LogicalBandwidthID",
                table: "BundleInterfaceVlan",
                column: "LogicalBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_InterfaceBandwidth_LogicalBandwidthID",
                table: "Interface",
                column: "LogicalBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_InterfaceBandwidth_LogicalBandwidthID",
                table: "InterfaceVlan",
                column: "LogicalBandwidthID",
                principalTable: "InterfaceBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
