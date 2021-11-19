using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class ChangeScanServerDocumentColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO_PACKAGE");

            migrationBuilder.AddColumn<string>(
                name: "SCAN_CHECKER_REQUEST_ID",
                table: "PH_PHOTO_PACKAGE_HISTORY",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SCAN_CHECKER_REQUEST_ID",
                table: "PH_PHOTO_PACKAGE_HISTORY");

            migrationBuilder.DropColumn(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO");

            migrationBuilder.AddColumn<string>(
                name: "SCAN_SERVER_DOCUMENT_ID",
                table: "PH_PHOTO_PACKAGE",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: true);
        }
    }
}
