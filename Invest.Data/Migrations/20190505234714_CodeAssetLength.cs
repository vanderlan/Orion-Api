using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class CodeAssetLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Asset",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Asset",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }
    }
}
