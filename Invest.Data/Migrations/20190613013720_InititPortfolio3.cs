using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class InititPortfolio3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "Investment",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Portfolio_PortfolioId",
                table: "Investment",
                column: "PortfolioId",
                principalTable: "Portfolio",
                principalColumn: "PortfolioId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "PortfolioId",
                table: "Investment",
                nullable: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Investment_Portfolio_PortfolioId",
                table: "Investment",
                column: "PortfolioId",
                principalTable: "Portfolio",
                principalColumn: "PortfolioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
