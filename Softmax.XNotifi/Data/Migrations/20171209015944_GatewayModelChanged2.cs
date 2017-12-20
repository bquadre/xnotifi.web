using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XNotifi.Data.Migrations
{
    public partial class GatewayModelChanged2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSecure",
                table: "Gateways");

            migrationBuilder.DropColumn(
                name: "Port",
                table: "Gateways");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSecure",
                table: "Gateways",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Port",
                table: "Gateways",
                nullable: false,
                defaultValue: 0);
        }
    }
}
