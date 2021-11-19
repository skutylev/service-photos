using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class AddScanServerDataToHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO_HISTORY",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SCAN_SERVER_PAGE_ID",
                table: "PH_PHOTO_HISTORY",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SCAN_SERVER_PAGE_NUMBER",
                table: "PH_PHOTO_HISTORY",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TRANZ_WARE_SCAN_ID",
                table: "PH_PHOTO_HISTORY",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO_HISTORY");

            migrationBuilder.DropColumn(
                name: "SCAN_SERVER_PAGE_ID",
                table: "PH_PHOTO_HISTORY");

            migrationBuilder.DropColumn(
                name: "SCAN_SERVER_PAGE_NUMBER",
                table: "PH_PHOTO_HISTORY");

            migrationBuilder.DropColumn(
                name: "TRANZ_WARE_SCAN_ID",
                table: "PH_PHOTO_HISTORY");
        }
    }
}
