using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class PublicIdPattern : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "StockQuotesHistory",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "StockExchange",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Sector",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "FundamentalistDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Asset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "StockQuotesHistory");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "StockExchange");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "FundamentalistDetail");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Asset");
        }
    }
}
