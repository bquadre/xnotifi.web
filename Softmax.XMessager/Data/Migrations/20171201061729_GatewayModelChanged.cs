using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Softmax.XMessager.Data.Migrations
{
    public partial class GatewayModelChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Gateways",
                newName: "ServiceUrl");

            migrationBuilder.RenameColumn(
                name: "Service",
                table: "Gateways",
                newName: "Port");

            migrationBuilder.AddColumn<bool>(
                name: "IsSecure",
                table: "Gateways",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProviderUrl",
                table: "Gateways",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceCode",
                table: "Gateways",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSecure",
                table: "Gateways");

            migrationBuilder.DropColumn(
                name: "ProviderUrl",
                table: "Gateways");

            migrationBuilder.DropColumn(
                name: "ServiceCode",
                table: "Gateways");

            migrationBuilder.RenameColumn(
                name: "ServiceUrl",
                table: "Gateways",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Port",
                table: "Gateways",
                newName: "Service");
        }
    }
}
