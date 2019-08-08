using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class RelationshipSectorAndExcahnge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockExchangeId",
                table: "Sector",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sector_StockExchangeId",
                table: "Sector",
                column: "StockExchangeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sector_StockExchange_StockExchangeId",
                table: "Sector",
                column: "StockExchangeId",
                principalTable: "StockExchange",
                principalColumn: "StockExchangeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sector_StockExchange_StockExchangeId",
                table: "Sector");

            migrationBuilder.DropIndex(
                name: "IX_Sector_StockExchangeId",
                table: "Sector");

            migrationBuilder.DropColumn(
                name: "StockExchangeId",
                table: "Sector");
        }
    }
}
