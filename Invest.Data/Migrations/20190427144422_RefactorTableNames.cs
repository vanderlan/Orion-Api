using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class RefactorTableNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.CreateTable(
                name: "StockQuotesHistory",
                columns: table => new
                {
                    StockQuotesHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    Open = table.Column<decimal>(nullable: false),
                    High = table.Column<decimal>(nullable: false),
                    Low = table.Column<decimal>(nullable: false),
                    Close = table.Column<decimal>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuotesHistory", x => x.StockQuotesHistoryId);
                    table.ForeignKey(
                        name: "FK_StockQuotesHistory_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockQuotesHistory_AssetId",
                table: "StockQuotesHistory",
                column: "AssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockQuotesHistory");

            migrationBuilder.CreateTable(
                name: "PriceHistory",
                columns: table => new
                {
                    PriceHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetId = table.Column<int>(nullable: false),
                    Close = table.Column<decimal>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    High = table.Column<decimal>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    Low = table.Column<decimal>(nullable: false),
                    Open = table.Column<decimal>(nullable: false),
                    Volume = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceHistory", x => x.PriceHistoryId);
                    table.ForeignKey(
                        name: "FK_PriceHistory_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_AssetId",
                table: "PriceHistory",
                column: "AssetId");
        }
    }
}
