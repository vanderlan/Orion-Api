using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class CreateFundamentalistTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundamentalistDetail");

            migrationBuilder.CreateTable(
                name: "Fundamentalist",
                columns: table => new
                {
                    FundamentalistId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    ROE = table.Column<decimal>(nullable: true),
                    PL = table.Column<decimal>(nullable: true),
                    AssetId = table.Column<int>(nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fundamentalist");

            migrationBuilder.CreateTable(
                name: "FundamentalistDetail",
                columns: table => new
                {
                    FundamentalistDetailId = table.Column<decimal>(nullable: false),
                    AssetId = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    PL = table.Column<decimal>(nullable: false),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    ROE = table.Column<decimal>(nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_FundamentalistDetail_AssetId",
                table: "FundamentalistDetail",
                column: "AssetId");
        }
    }
}
