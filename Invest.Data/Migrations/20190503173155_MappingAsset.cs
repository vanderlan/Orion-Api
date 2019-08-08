using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class MappingAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_StockExchange_StockExchangeId",
                table: "Asset");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Asset",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Asset",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Asset",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset",
                column: "SectorId",
                principalTable: "Sector",
                principalColumn: "SectorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_StockExchange_StockExchangeId",
                table: "Asset",
                column: "StockExchangeId",
                principalTable: "StockExchange",
                principalColumn: "StockExchangeId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_Asset_StockExchange_StockExchangeId",
                table: "Asset");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Asset",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Asset",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Asset",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset",
                column: "SectorId",
                principalTable: "Sector",
                principalColumn: "SectorId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_StockExchange_StockExchangeId",
                table: "Asset",
                column: "StockExchangeId",
                principalTable: "StockExchange",
                principalColumn: "StockExchangeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
