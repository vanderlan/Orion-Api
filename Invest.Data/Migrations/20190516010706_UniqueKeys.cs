using Microsoft.EntityFrameworkCore.Migrations;

namespace Invest.Data.Migrations
{
    public partial class UniqueKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_User_UserId",
                table: "Subscription");

            migrationBuilder.DropIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Subscription");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 300);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptionId",
                table: "User",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Email",
                table: "User",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_SubscriptionId",
                table: "User",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_StockExchange_Name",
                table: "StockExchange",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sector_Name",
                table: "Sector",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asset_Code",
                table: "Asset",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_User_Subscription_SubscriptionId",
                table: "User",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "SubscriptionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Subscription_SubscriptionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_Email",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_SubscriptionId",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_StockExchange_Name",
                table: "StockExchange");

            migrationBuilder.DropIndex(
                name: "IX_Sector_Name",
                table: "Sector");

            migrationBuilder.DropIndex(
                name: "IX_Asset_Code",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "SubscriptionId",
                table: "User");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Subscription",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserId",
                table: "Subscription",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_User_UserId",
                table: "Subscription",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
