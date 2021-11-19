using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Photos.Dal.Migrations
{
    public partial class AddUserSapNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<long>(
                name: "USER_SAP_NUMBER",
                table: "PACKAGE_PHOTO_HISTORY",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "USER_SAP_NUMBER",
                table: "PACKAGE_PHOTO",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "PHOTO_TYPE_ID",
                table: "APPLICATION_PHOTO_HISTORY",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PHOTO_TYPE_ID",
                table: "APPLICATION_PHOTO",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "USER_SAP_NUMBER",
                table: "PACKAGE_PHOTO_HISTORY");

            migrationBuilder.DropColumn(
                name: "USER_SAP_NUMBER",
                table: "PACKAGE_PHOTO");

            migrationBuilder.AlterColumn<int>(
                name: "PHOTO_TYPE_ID",
                table: "APPLICATION_PHOTO_HISTORY",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PHOTO_TYPE_ID",
                table: "APPLICATION_PHOTO",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
