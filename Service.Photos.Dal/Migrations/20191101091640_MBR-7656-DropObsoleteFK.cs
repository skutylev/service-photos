using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Photos.Dal.Migrations
{
    public partial class MBR7656DropObsoleteFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"alter table PACKAGE_PHOTO
                drop constraint FK_PACKAGE_PHOTO_APPL_ID
                /");

            migrationBuilder.Sql(@" alter table PACKAGE_PHOTO
                drop constraint FK_PACKAGE_PHOTO_USER_ID
                    /");

            migrationBuilder.Sql(@"alter table PACKAGE_PHOTO
              drop constraint FK_PACKAGE_PHOTO_USER_USER_ID
            /");
        }
    }
}
