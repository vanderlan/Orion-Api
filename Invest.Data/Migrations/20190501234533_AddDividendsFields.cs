using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class AddDividendsFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AdjustedClose",
                table: "StockQuotesHistory",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Dividend",
                table: "StockQuotesHistory",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Split",
                table: "StockQuotesHistory",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdjustedClose",
                table: "StockQuotesHistory");

            migrationBuilder.DropColumn(
                name: "Dividend",
                table: "StockQuotesHistory");

            migrationBuilder.DropColumn(
                name: "Split",
                table: "StockQuotesHistory");
        }
    }
}
