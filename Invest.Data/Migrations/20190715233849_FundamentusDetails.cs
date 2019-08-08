using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class FundamentusDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fundamentalist");

            migrationBuilder.CreateTable(
                name: "FundamentalistIndicators",
                columns: table => new
                {
                    FundamentalistIndicatorsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ROE = table.Column<decimal>(nullable: true),
                    PL = table.Column<decimal>(nullable: true),
                    AssetId = table.Column<int>(nullable: false),
                    LPA = table.Column<decimal>(nullable: true),
                    MargBruta = table.Column<decimal>(nullable: true),
                    MargEBIT = table.Column<decimal>(nullable: true),
                    MargLiquida = table.Column<decimal>(nullable: true),
                    EBITporAtivo = table.Column<decimal>(nullable: true),
                    ROIC = table.Column<decimal>(nullable: true),
                    LiquidezCorr = table.Column<decimal>(nullable: true),
                    DivBrporPatrim = table.Column<decimal>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundamentalistIndicators", x => x.FundamentalistIndicatorsId);
                    table.ForeignKey(
                        name: "FK_FundamentalistIndicators_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FundamentalistIndicators_AssetId",
                table: "FundamentalistIndicators",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FundamentalistIndicators_Date_AssetId",
                table: "FundamentalistIndicators",
                columns: new[] { "Date", "AssetId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundamentalistIndicators");

            migrationBuilder.CreateTable(
                name: "Fundamentalist",
                columns: table => new
                {
                    FundamentalistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssetId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PL = table.Column<decimal>(nullable: true),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    ROE = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fundamentalist", x => x.FundamentalistId);
                    table.ForeignKey(
                        name: "FK_Fundamentalist_Asset_AssetId",
                        column: x => x.AssetId,
                        principalTable: "Asset",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fundamentalist_AssetId",
                table: "Fundamentalist",
                column: "AssetId");
        }
    }
}
