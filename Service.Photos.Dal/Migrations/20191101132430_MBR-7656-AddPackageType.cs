using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Photos.Dal.Migrations
{
    public partial class MBR7656AddPackageType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TYPE",
                table: "PACKAGE_PHOTO",
                nullable: false,
                defaultValue: 1);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TYPE",
                table: "PACKAGE_PHOTO");
        }
    }
}
