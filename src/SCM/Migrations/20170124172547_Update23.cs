using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetwork_BgpPeer_BgpPeerID",
                table: "TenantNetwork");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                table: "TenantNetworkBgpPeer");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                table: "TenantNetworkBgpPeer");

            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_BgpPeerID",
                table: "TenantNetwork");

            migrationBuilder.DropColumn(
                name: "BgpPeerID",
                table: "TenantNetwork");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                table: "TenantNetworkBgpPeer",
                column: "BgpPeerID",
                principalTable: "BgpPeer",
                principalColumn: "BgpPeerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                column: "TenantNetworkID",
                principalTable: "TenantNetwork",
                principalColumn: "TenantNetworkID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                table: "TenantNetworkBgpPeer");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                table: "TenantNetworkBgpPeer");

            migrationBuilder.AddColumn<int>(
                name: "BgpPeerID",
                table: "TenantNetwork",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_BgpPeerID",
                table: "TenantNetwork",
                column: "BgpPeerID");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetwork_BgpPeer_BgpPeerID",
                table: "TenantNetwork",
                column: "BgpPeerID",
                principalTable: "BgpPeer",
                principalColumn: "BgpPeerID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetworkBgpPeer_BgpPeer_BgpPeerID",
                table: "TenantNetworkBgpPeer",
                column: "BgpPeerID",
                principalTable: "BgpPeer",
                principalColumn: "BgpPeerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantNetworkBgpPeer_TenantNetwork_TenantNetworkID",
                table: "TenantNetworkBgpPeer",
                column: "TenantNetworkID",
                principalTable: "TenantNetwork",
                principalColumn: "TenantNetworkID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
