using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class HistoryQuotesUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "StockQuotesHistory",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockQuotesHistory_Date_AssetId",
                table: "StockQuotesHistory",
                columns: new[] { "Date", "AssetId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockQuotesHistory_Date_AssetId",
                table: "StockQuotesHistory");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "StockQuotesHistory",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
