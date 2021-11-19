using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class AddPhotoStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "STATUS_CODE",
                table: "PH_PHOTO_HISTORY",
                type: "nvarchar2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "STATUS_CODE",
                table: "PH_PHOTO",
                type: "nvarchar2(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "STATUS_CODE",
                table: "PH_PHOTO_HISTORY");

            migrationBuilder.DropColumn(
                name: "STATUS_CODE",
                table: "PH_PHOTO");
        }
    }
}
