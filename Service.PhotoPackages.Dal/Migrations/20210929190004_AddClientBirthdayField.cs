using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class AddClientBirthdayField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CLIENT_BIRTHDAY",
                table: "PH_PHOTO_PACKAGE",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CLIENT_BIRTHDAY",
                table: "PH_PHOTO_PACKAGE");
        }
    }
}
