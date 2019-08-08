using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Invest.Data.Migrations
{
    public partial class CompanyModelB3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_FundamentalistDetail_Asset_AssetId",
                table: "FundamentalistDetail");

            migrationBuilder.DropColumn(
                name: "Details",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Asset");

            migrationBuilder.DropColumn(
                name: "Site",
                table: "Asset");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                table: "FundamentalistDetail",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_FundamentalistDetail_AssetId",
                table: "FundamentalistDetail",
                newName: "IX_FundamentalistDetail_CompanyId");

            migrationBuilder.RenameColumn(
                name: "SectorId",
                table: "Asset",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_SectorId",
                table: "Asset",
                newName: "IX_Asset_CompanyId");

            migrationBuilder.CreateTable(
                name: "SubSector",
                columns: table => new
                {
                    SubSectorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SectorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubSector", x => x.SubSectorId);
                    table.ForeignKey(
                        name: "FK_SubSector_Sector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sector",
                        principalColumn: "SectorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Segment",
                columns: table => new
                {
                    SegmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SubSectorId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segment", x => x.SegmentId);
                    table.ForeignKey(
                        name: "FK_Segment_SubSector_SubSectorId",
                        column: x => x.SubSectorId,
                        principalTable: "SubSector",
                        principalColumn: "SubSectorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PublicId = table.Column<string>(nullable: true),
                    LastUpdated = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Details = table.Column<string>(nullable: true),
                    Site = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: true),
                    SectorId = table.Column<int>(nullable: false),
                    SubSectorId = table.Column<int>(nullable: true),
                    SegmentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Company_Sector_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sector",
                        principalColumn: "SectorId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Company_Segment_SegmentId",
                        column: x => x.SegmentId,
                        principalTable: "Segment",
                        principalColumn: "SegmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Company_SubSector_SubSectorId",
                        column: x => x.SubSectorId,
                        principalTable: "SubSector",
                        principalColumn: "SubSectorId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Company_SectorId",
                table: "Company",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_SegmentId",
                table: "Company",
                column: "SegmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Company_SubSectorId",
                table: "Company",
                column: "SubSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Segment_SubSectorId",
                table: "Segment",
                column: "SubSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_SubSector_SectorId",
                table: "SubSector",
                column: "SectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Company_CompanyId",
                table: "Asset",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundamentalistDetail_Company_CompanyId",
                table: "FundamentalistDetail",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Asset_Company_CompanyId",
                table: "Asset");

            migrationBuilder.DropForeignKey(
                name: "FK_FundamentalistDetail_Company_CompanyId",
                table: "FundamentalistDetail");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "Segment");

            migrationBuilder.DropTable(
                name: "SubSector");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "FundamentalistDetail",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_FundamentalistDetail_CompanyId",
                table: "FundamentalistDetail",
                newName: "IX_FundamentalistDetail_AssetId");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                table: "Asset",
                newName: "SectorId");

            migrationBuilder.RenameIndex(
                name: "IX_Asset_CompanyId",
                table: "Asset",
                newName: "IX_Asset_SectorId");

            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Site",
                table: "Asset",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Asset_Sector_SectorId",
                table: "Asset",
                column: "SectorId",
                principalTable: "Sector",
                principalColumn: "SectorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FundamentalistDetail_Asset_AssetId",
                table: "FundamentalistDetail",
                column: "AssetId",
                principalTable: "Asset",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
