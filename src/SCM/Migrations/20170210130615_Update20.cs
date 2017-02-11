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
                name: "FK_AttachmentSetVrf_Vrfs_VrfID",
                table: "AttachmentSetVrf");

            migrationBuilder.DropForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Vrfs_VrfID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_Vrfs_VrfID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID",
                table: "InterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrfs_Device_DeviceID",
                table: "Vrfs");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrfs_Tenant_TenantID",
                table: "Vrfs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vrfs",
                table: "Vrfs");

            migrationBuilder.RenameTable(
                name: "Vrfs",
                newName: "Vrf");

            migrationBuilder.RenameIndex(
                name: "IX_Vrfs_AdministratorSubField_AssignedNumberSubField",
                table: "Vrf",
                newName: "IX_Vrf_AdministratorSubField_AssignedNumberSubField");

            migrationBuilder.RenameIndex(
                name: "IX_Vrfs_TenantID",
                table: "Vrf",
                newName: "IX_Vrf_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Vrfs_Name",
                table: "Vrf",
                newName: "IX_Vrf_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Vrfs_DeviceID",
                table: "Vrf",
                newName: "IX_Vrf_DeviceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vrf",
                table: "Vrf",
                column: "VrfID");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BgpPeer_Vrf_VrfID",
                table: "BgpPeer",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Vrf_VrfID",
                table: "BundleInterface",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_Vrf_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_Vrf_VrfID",
                table: "Interface",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID",
                table: "InterfaceVlan",
                column: "VrfID",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1",
                principalTable: "Vrf",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrf_Tenant_TenantID",
                table: "Vrf",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttachmentSetVrf_Vrf_VrfID",
                table: "AttachmentSetVrf");

            migrationBuilder.DropForeignKey(
                name: "FK_BgpPeer_Vrf_VrfID",
                table: "BgpPeer");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterface_Vrf_VrfID",
                table: "BundleInterface");

            migrationBuilder.DropForeignKey(
                name: "FK_BundleInterfaceVlan_Vrf_VrfID",
                table: "BundleInterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Interface_Vrf_VrfID",
                table: "Interface");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID",
                table: "InterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_InterfaceVlan_Vrf_VrfID1",
                table: "InterfaceVlan");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Device_DeviceID",
                table: "Vrf");

            migrationBuilder.DropForeignKey(
                name: "FK_Vrf_Tenant_TenantID",
                table: "Vrf");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vrf",
                table: "Vrf");

            migrationBuilder.RenameTable(
                name: "Vrf",
                newName: "Vrfs");

            migrationBuilder.RenameIndex(
                name: "IX_Vrf_AdministratorSubField_AssignedNumberSubField",
                table: "Vrfs",
                newName: "IX_Vrfs_AdministratorSubField_AssignedNumberSubField");

            migrationBuilder.RenameIndex(
                name: "IX_Vrf_TenantID",
                table: "Vrfs",
                newName: "IX_Vrfs_TenantID");

            migrationBuilder.RenameIndex(
                name: "IX_Vrf_Name",
                table: "Vrfs",
                newName: "IX_Vrfs_Name");

            migrationBuilder.RenameIndex(
                name: "IX_Vrf_DeviceID",
                table: "Vrfs",
                newName: "IX_Vrfs_DeviceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vrfs",
                table: "Vrfs",
                column: "VrfID");

            migrationBuilder.AddForeignKey(
                name: "FK_AttachmentSetVrf_Vrfs_VrfID",
                table: "AttachmentSetVrf",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BgpPeer_Vrfs_VrfID",
                table: "BgpPeer",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterface_Vrfs_VrfID",
                table: "BundleInterface",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleInterfaceVlan_Vrfs_VrfID",
                table: "BundleInterfaceVlan",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Interface_Vrfs_VrfID",
                table: "Interface",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID",
                table: "InterfaceVlan",
                column: "VrfID",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterfaceVlan_Vrfs_VrfID1",
                table: "InterfaceVlan",
                column: "VrfID1",
                principalTable: "Vrfs",
                principalColumn: "VrfID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrfs_Device_DeviceID",
                table: "Vrfs",
                column: "DeviceID",
                principalTable: "Device",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vrfs_Tenant_TenantID",
                table: "Vrfs",
                column: "TenantID",
                principalTable: "Tenant",
                principalColumn: "TenantID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
