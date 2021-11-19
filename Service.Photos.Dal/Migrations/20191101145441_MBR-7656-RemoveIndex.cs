using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Photos.Dal.Migrations
{
    public partial class MBR7656RemoveIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PACKAGE_PHOTO_APPL_ID",
                table: "PACKAGE_PHOTO");

            migrationBuilder.CreateIndex(
                name: "IX_PACKAGE_PHOTO_APPL_ID_TYPE",
                table: "PACKAGE_PHOTO",
                columns: new[] { "APPL_ID", "TYPE" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PACKAGE_PHOTO_APPL_ID_TYPE",
                table: "PACKAGE_PHOTO");
        }
    }
}
