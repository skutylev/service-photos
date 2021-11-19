using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PH_PHOTO_HISTORY",
                columns: table => new
                {
                    ID = table.Column<long>(type: "int64", nullable: false)
                        .Annotation("Devart.Data.Oracle:Autoincrement", true),
                    PHOTO_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    CONTENT_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    THUMBNAIL_CONTENT_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    ERROR_CODE = table.Column<string>(type: "nvarchar2(100)", maxLength: 100, nullable: true),
                    ERROR_MESSAGE = table.Column<string>(type: "nvarchar2(1000)", maxLength: 1000, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PH_PHOTO_HISTORY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PH_PHOTO_PACKAGE",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    BUCKET_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    TYPE_CODE = table.Column<string>(type: "nvarchar2(50)", maxLength: 50, nullable: false),
                    CONTRACT_ID = table.Column<long>(type: "int64", nullable: false),
                    STATUS_CODE = table.Column<string>(type: "nvarchar2(50)", maxLength: 50, nullable: false),
                    PROCESS_CODE = table.Column<string>(type: "nvarchar2(100)", maxLength: 100, nullable: true),
                    AUTHOR_SAP_NUMBER = table.Column<long>(type: "int64", nullable: true),
                    VERIFIER_SAP_NUMBER = table.Column<long>(type: "int64", nullable: true),
                    SCAN_CHECKER_REQUEST_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    SCAN_SERVER_DOCUMENT_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    LATITUDE = table.Column<double>(type: "binary_double", nullable: true),
                    LONGITUDE = table.Column<double>(type: "binary_double", nullable: true),
                    ERROR_CODE = table.Column<string>(type: "nvarchar2(100)", maxLength: 100, nullable: true),
                    ERROR_MESSAGE = table.Column<string>(type: "nvarchar2(1000)", maxLength: 1000, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "date", nullable: false),
                    UPDATE_DATE = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PH_PHOTO_PACKAGE", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PH_PHOTO_PACKAGE_HISTORY",
                columns: table => new
                {
                    ID = table.Column<long>(type: "int64", nullable: false)
                        .Annotation("Devart.Data.Oracle:Autoincrement", true),
                    PHOTO_PACKAGE_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    STATUS_CODE = table.Column<string>(type: "nvarchar2(50)", maxLength: 50, nullable: false),
                    AUTHOR_SAP_NUMBER = table.Column<long>(type: "int64", nullable: true),
                    VERIFIER_SAP_NUMBER = table.Column<long>(type: "int64", nullable: true),
                    LATITUDE = table.Column<double>(type: "binary_double", nullable: true),
                    LONGITUDE = table.Column<double>(type: "binary_double", nullable: true),
                    ERROR_CODE = table.Column<string>(type: "nvarchar2(100)", maxLength: 100, nullable: true),
                    ERROR_MESSAGE = table.Column<string>(type: "nvarchar2(1000)", maxLength: 1000, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PH_PHOTO_PACKAGE_HISTORY", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PH_PHOTO",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    PHOTO_PACKAGE_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: false),
                    CONTENT_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    THUMBNAIL_CONTENT_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    TYPE_CODE = table.Column<string>(type: "nvarchar2(50)", maxLength: 50, nullable: false),
                    ATTACH_SYSTEM_CODE = table.Column<string>(type: "nvarchar2(50)", maxLength: 50, nullable: false),
                    SCAN_SERVER_PAGE_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    SCAN_SERVER_PAGE_NUMBER = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    TRANZ_WARE_SCAN_ID = table.Column<string>(type: "nvarchar2(36)", maxLength: 36, nullable: true),
                    ERROR_CODE = table.Column<string>(type: "nvarchar2(100)", maxLength: 100, nullable: true),
                    ERROR_MESSAGE = table.Column<string>(type: "nvarchar2(1000)", maxLength: 1000, nullable: true),
                    START_DATE = table.Column<DateTime>(type: "date", nullable: false),
                    UPDATE_DATE = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PH_PHOTO", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PH_PHOTO_PH_PHOTO_PACKAGE_PHOTO_PACKAGE_ID",
                        column: x => x.PHOTO_PACKAGE_ID,
                        principalTable: "PH_PHOTO_PACKAGE",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PH_PHOTO_PHOTO_PACKAGE_ID",
                table: "PH_PHOTO",
                column: "PHOTO_PACKAGE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PH_PHOTO_HISTORY_PHOTO_ID",
                table: "PH_PHOTO_HISTORY",
                column: "PHOTO_ID");

            migrationBuilder.CreateIndex(
                name: "IX_PH_PHOTO_PACKAGE_HISTORY_PHOTO_PACKAGE_ID",
                table: "PH_PHOTO_PACKAGE_HISTORY",
                column: "PHOTO_PACKAGE_ID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PH_PHOTO");

            migrationBuilder.DropTable(
                name: "PH_PHOTO_HISTORY");

            migrationBuilder.DropTable(
                name: "PH_PHOTO_PACKAGE_HISTORY");

            migrationBuilder.DropTable(
                name: "PH_PHOTO_PACKAGE");
        }
    }
}
