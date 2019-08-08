using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sector",
                columns: table => new
                {
                    SectorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sector", x => x.SectorId);
                });

            migrationBuilder.CreateTable(
                name: "StockExchange",
                columns: table => new
                {
                    StockExchangeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockExchange", x => x.StockExchangeId);
                });

            migrationBuilder.CreateTable(
                name: "Asset",
                columns: table => new
                {
                    AssetId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    StockExchangeId = table.Column<int>(nullable: false),
                    SectorId = table.Column<int>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asset", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_Asset_Sector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sector",
                        principalColumn: "SectorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Asset_StockExchange_StockExchangeId",
                        column: x => x.StockExchangeId,
                        principalTable: "StockExchange",
                        principalColumn: "StockExchangeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FundamentalistDetail",
                columns: table => new
                {
                    FundamentalistDetailId = table.Column<decimal>(nullable: false),
                    ROE = table.Column<decimal>(nullable: false),
                    PL = table.Column<decimal>(nullable: false),
                    AssetId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundamentalistDetail", x => x.FundamentalistDetailId);
                    table.ForeignKey(
                        name: "FK_FundamentalistDetail_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceHistory",
                columns: table => new
                {
                    PriceHistoryId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Open = table.Column<decimal>(nullable: false),
                    High = table.Column<decimal>(nullable: false),
                    Low = table.Column<decimal>(nullable: false),
                    Close = table.Column<decimal>(nullable: false),
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
                name: "IX_Asset_SectorId",
                table: "Asset",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Asset_StockExchangeId",
                table: "Asset",
                column: "StockExchangeId");

            migrationBuilder.CreateIndex(
                name: "IX_FundamentalistDetail_AssetId",
                table: "FundamentalistDetail",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceHistory_AssetId",
                table: "PriceHistory",
                column: "AssetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundamentalistDetail");

            migrationBuilder.DropTable(
                name: "PriceHistory");

            migrationBuilder.DropTable(
                name: "Asset");

            migrationBuilder.DropTable(
                name: "Sector");

            migrationBuilder.DropTable(
                name: "StockExchange");
        }
    }
}
