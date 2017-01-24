using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_LogicalBandwidth_LogicalBandwidthID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_LogicalBandwidth_LogicalBandwidthID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                table: "InterfaceVlan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LogicalBandwidth",
                table: "LogicalBandwidth");

            migrationBuilder.DropColumn(
                name: "PhysicalPortBandwidthID",
                table: "Port");

            migrationBuilder.RenameTable(
                name: "LogicalBandwidth",
                newName: "InterfaceBandwidth");

            migrationBuilder.RenameIndex(
                name: "IX_LogicalBandwidth_BandwidthKbps",
                table: "InterfaceBandwidth",
                newName: "IX_InterfaceBandwidth_BandwidthKbps");

            migrationBuilder.AlterColumn<int>(
                name: "PortBandwidthID",
                table: "Port",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_InterfaceBandwidth",
                table: "InterfaceBandwidth",
                column: "InterfaceBandwidthID");

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_InterfaceBandwidth",
                table: "InterfaceBandwidth");

            migrationBuilder.RenameTable(
                name: "InterfaceBandwidth",
                newName: "LogicalBandwidth");

            migrationBuilder.RenameIndex(
                name: "IX_InterfaceBandwidth_BandwidthKbps",
                table: "LogicalBandwidth",
                newName: "IX_LogicalBandwidth_BandwidthKbps");

            migrationBuilder.AlterColumn<int>(
                name: "PortBandwidthID",
                table: "Port",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "PhysicalPortBandwidthID",
                table: "Port",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LogicalBandwidth",
                table: "LogicalBandwidth",
                column: "InterfaceBandwidthID");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_LogicalBandwidth_LogicalBandwidthID",
                table: "BundleInterface",
                column: "LogicalBandwidthID",
                principalTable: "LogicalBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                table: "BundleInterfaceVlan",
                column: "LogicalBandwidthID",
                principalTable: "LogicalBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_LogicalBandwidth_LogicalBandwidthID",
                table: "Interface",
                column: "LogicalBandwidthID",
                principalTable: "LogicalBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_LogicalBandwidth_LogicalBandwidthID",
                table: "InterfaceVlan",
                column: "LogicalBandwidthID",
                principalTable: "LogicalBandwidth",
                principalColumn: "InterfaceBandwidthID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
