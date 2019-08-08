using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class DefaultDateTimes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockExchange",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "StockExchange",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Sector",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "Sector",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "PriceHistory",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "FundamentalistDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "FundamentalistDetail",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Asset",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockExchange");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "StockExchange");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "PriceHistory");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FundamentalistDetail");

            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "FundamentalistDetail");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Asset");
        }
    }
}
