using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class Addb3Id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "B3Id",
                table: "Company",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "B3Id",
                table: "Company");
        }
    }
}
