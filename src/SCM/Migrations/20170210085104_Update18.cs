using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SCM.Migrations
{
    public partial class Update18 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_TenantID_IpPrefix_Length",
                table: "TenantNetwork");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_TenantID",
                table: "TenantNetwork",
                column: "TenantID");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_IpPrefix_Length",
                table: "TenantNetwork",
                columns: new[] { "IpPrefix", "Length" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_TenantID",
                table: "TenantNetwork");

            migrationBuilder.DropIndex(
                name: "IX_TenantNetwork_IpPrefix_Length",
                table: "TenantNetwork");

            migrationBuilder.CreateIndex(
                name: "IX_TenantNetwork_TenantID_IpPrefix_Length",
                table: "TenantNetwork",
                columns: new[] { "TenantID", "IpPrefix", "Length" },
                unique: true);
        }
    }
}
