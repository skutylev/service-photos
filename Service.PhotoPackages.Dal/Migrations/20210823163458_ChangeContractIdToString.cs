using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class ChangeContractIdToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CONTRACT_ID",
                table: "PH_PHOTO_PACKAGE",
                type: "nvarchar2(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "int64");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CONTRACT_ID",
                table: "PH_PHOTO_PACKAGE",
                type: "int64",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar2(36)",
                oldMaxLength: 36);
        }
    }
}
