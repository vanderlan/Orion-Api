using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class InititPortfolio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Investment",
                columns: table => new
                {
                    InvestmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    Unities = table.Column<int>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investment", x => x.InvestmentId);
                    table.ForeignKey(
                        name: "FK_Investment_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Portfolio",
                columns: table => new
                {
                    PortfolioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolio", x => x.PortfolioId);
                    table.ForeignKey(
                        name: "FK_Portfolio_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investment_AssetId",
                table: "Investment",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_Portfolio_UserId",
                table: "Portfolio",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investment");

            migrationBuilder.DropTable(
                name: "Portfolio");
        }
    }
}
