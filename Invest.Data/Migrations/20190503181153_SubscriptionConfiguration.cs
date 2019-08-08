using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class SubscriptionConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "StockExchange",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StockExchange",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Sector",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sector",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "FundamentalistDetail",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: false),
                    LastName = table.Column<string>(maxLength: 100, nullable: false),
                    Email = table.Column<string>(maxLength: 300, nullable: false),
                    Password = table.Column<string>(maxLength: 100, nullable: false),
                    Profile = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscriptionId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(maxLength: 50, nullable: false),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    SubscriptionStatus = table.Column<int>(nullable: false),
                    SubscriptionType = table.Column<int>(nullable: false),
                    ExpirationTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscription", x => x.SubscriptionId);
                    table.ForeignKey(
                        name: "FK_Subscription_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "StockExchange",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "StockExchange",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "Sector",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Sector",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "PublicId",
                table: "FundamentalistDetail",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);
        }
    }
}
