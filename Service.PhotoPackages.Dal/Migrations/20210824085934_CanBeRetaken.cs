using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class CanBeRetaken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CAN_BE_RETAKEN",
                table: "PH_PHOTO",
                type: "bool",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CAN_BE_RETAKEN",
                table: "PH_PHOTO");
        }
    }
}
