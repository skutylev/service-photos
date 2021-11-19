using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class HistoryLinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "REQUIRED",
                table: "PH_PHOTO",
                type: "bool",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_PH_PHOTO_HISTORY_PH_PHOTO_PHOTO_ID",
                table: "PH_PHOTO_HISTORY",
                column: "PHOTO_ID",
                principalTable: "PH_PHOTO",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PH_PHOTO_PACKAGE_HISTORY_PH_PHOTO_PACKAGE_PHOTO_PACKAGE_ID",
                table: "PH_PHOTO_PACKAGE_HISTORY",
                column: "PHOTO_PACKAGE_ID",
                principalTable: "PH_PHOTO_PACKAGE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PH_PHOTO_HISTORY_PH_PHOTO_PHOTO_ID",
                table: "PH_PHOTO_HISTORY");

            migrationBuilder.DropForeignKey(
                name: "FK_PH_PHOTO_PACKAGE_HISTORY_PH_PHOTO_PACKAGE_PHOTO_PACKAGE_ID",
                table: "PH_PHOTO_PACKAGE_HISTORY");

            migrationBuilder.DropColumn(
                name: "REQUIRED",
                table: "PH_PHOTO");
        }
    }
}
