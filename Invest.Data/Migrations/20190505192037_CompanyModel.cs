using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class CompanyModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundamentalistDetail_Company_CompanyId",
                table: "FundamentalistDetail");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "FundamentalistDetail",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_FundamentalistDetail_CompanyId",
                table: "FundamentalistDetail",
                newName: "IX_FundamentalistDetail_AssetId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Segment",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cnpj",
                table: "Company",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FundamentalistDetail_Asset_AssetId",
                table: "FundamentalistDetail",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FundamentalistDetail_Asset_AssetId",
                table: "FundamentalistDetail");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Segment");

            migrationBuilder.DropColumn(
                name: "Cnpj",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "FundamentalistDetail",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_FundamentalistDetail_AssetId",
                table: "FundamentalistDetail",
                newName: "IX_FundamentalistDetail_CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_FundamentalistDetail_Company_CompanyId",
                table: "FundamentalistDetail",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
