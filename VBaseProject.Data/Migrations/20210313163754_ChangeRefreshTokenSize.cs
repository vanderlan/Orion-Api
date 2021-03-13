using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VBaseProject.Data.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class ChangeRefreshTokenSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Refreshtoken",
                table: "RefreshToken",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated", "PublicId" },
                values: new object[] { new DateTime(2021, 3, 13, 13, 37, 53, 742, DateTimeKind.Local).AddTicks(9943), new DateTime(2021, 3, 13, 13, 37, 53, 744, DateTimeKind.Local).AddTicks(5149), "9f2e9852-5d9e-469b-9508-c5532649ae5d" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Refreshtoken",
                table: "RefreshToken",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "LastUpdated", "PublicId" },
                values: new object[] { new DateTime(2021, 3, 11, 9, 45, 39, 286, DateTimeKind.Local).AddTicks(4313), new DateTime(2021, 3, 11, 9, 45, 39, 287, DateTimeKind.Local).AddTicks(9484), "b6341581-51e5-461d-9e52-b2911b2333a8" });
        }
    }
}
