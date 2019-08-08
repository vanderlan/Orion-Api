using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class CompanyMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Company",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Company_Cnpj",
                table: "Company",
                column: "Cnpj",
                unique: true,
                filter: "[Cnpj] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Company_Cnpj",
                table: "Company");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Cnpj",
                table: "Company",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
