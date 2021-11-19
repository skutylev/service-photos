using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.Photos.Dal.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
//            migrationBuilder.CreateTable(
//                name: "CURRENT_PHOTO_REQUIRED",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    PHOTO_TYPE_ID = table.Column<int>(nullable: false),
//                    CREDIT_TYPE_KEY = table.Column<int>(nullable: false),
//                    IS_REQUIRED = table.Column<bool>(nullable: false),
//                    VISIBILITY = table.Column<bool>(nullable: false),
//                    IS_REMOTE_SIGN = table.Column<bool>(nullable: true),
//                    IS_DIGITAL = table.Column<bool>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_CURRENT_PHOTO_REQUIRED", x => x.ID);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "CURRENT_PHOTO_TYPE",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    TYPE_ID = table.Column<int>(nullable: false),
//                    PARENT_ID = table.Column<int>(nullable: true),
//                    CODE = table.Column<string>(type: "NVARCHAR2", maxLength: 255, nullable: false),
//                    NAME = table.Column<string>(type: "NVARCHAR2", maxLength: 255, nullable: false),
//                    ORDER = table.Column<int>(nullable: false),
//                    DOC_TYPE_ID = table.Column<int>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_CURRENT_PHOTO_TYPE", x => x.ID);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "PACKAGE_PHOTO",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    PACKAGE_NUMBER = table.Column<int>(nullable: false),
//                    PACKAGE_URL = table.Column<string>(type: "NVARCHAR2", maxLength: 1000, nullable: false),
//                    SEND_PACKAGE_DATE = table.Column<DateTime>(nullable: true),
//                    APROVE_DATE = table.Column<DateTime>(nullable: true),
//                    ERROR_DATE = table.Column<DateTime>(nullable: true),
//                    ERROR_FIX_DATE = table.Column<DateTime>(nullable: true),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    UPDATE_DATE = table.Column<DateTime>(nullable: true),
//                    PACKAGE_STATUS_KEY = table.Column<int>(nullable: true),
//                    IS_PHOTO_SEND = table.Column<bool>(nullable: false),
//                    REQUEST_ID = table.Column<Guid>(nullable: true),
//                    ERROR_TEXT = table.Column<string>(maxLength: 2000, nullable: true),
//                    ERROR_MESSAGE = table.Column<string>(maxLength: 2000, nullable: true),
//                    USER_ID = table.Column<long>(nullable: false),
//                    APPL_ID = table.Column<long>(nullable: false)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PACKAGE_PHOTO", x => x.ID);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "PACKAGE_PHOTO_HISTORY",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    END_DATE = table.Column<DateTime>(nullable: true),
//                    PACKAGE_ID = table.Column<long>(nullable: false),
//                    APPROVE_DATE = table.Column<DateTime>(nullable: true),
//                    ERROR_DATE = table.Column<DateTime>(nullable: true),
//                    FULL_NAME = table.Column<string>(type: "NVARCHAR2", maxLength: 365, nullable: true),
//                    SAP_NUMBER = table.Column<int>(nullable: false),
//                    PACKAGE_STATUS_KEY = table.Column<int>(nullable: true),
//                    SEND_PACKAGE_DATE = table.Column<DateTime>(nullable: true),
//                    PACKAGE_NUMBER = table.Column<int>(nullable: false),
//                    USER_ID = table.Column<long>(nullable: true),
//                    REQUEST_ID = table.Column<Guid>(nullable: true),
//                    ERROR_TEXT = table.Column<string>(maxLength: 2000, nullable: true),
//                    ERROR_MESSAGE = table.Column<string>(maxLength: 2000, nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_PACKAGE_PHOTO_HISTORY", x => x.ID);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "APPLICATION_PHOTO_HISTORY",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    ERROR_TEXT = table.Column<string>(type: "NVARCHAR2", maxLength: 1000, nullable: true),
//                    ERROR_CODE = table.Column<string>(maxLength: 1000, nullable: true),
//                    IS_DELETED = table.Column<bool>(nullable: true),
//                    LATITUDE = table.Column<double>(nullable: false),
//                    LONGITUDE = table.Column<double>(nullable: false),
//                    FOUND_DATE = table.Column<DateTime>(nullable: false),
//                    PHOTO_TYPE_ID = table.Column<int>(nullable: true),
//                    PARENT_PHOTO_TYPE_ID = table.Column<int>(nullable: true),
//                    PACKAGE_HISTORY_ID = table.Column<long>(nullable: false),
//                    PhotoPackageHistoryId = table.Column<long>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_APPLICATION_PHOTO_HISTORY", x => x.ID);
//                    table.ForeignKey(
//                        name: "FK_APPLICATION_PHOTO_HISTORY_PACKAGE_PHOTO_HISTORY_PACKAGE_HISTORY_ID",
//                        column: x => x.PACKAGE_HISTORY_ID,
//                        principalTable: "PACKAGE_PHOTO_HISTORY",
//                        principalColumn: "ID",
//                        onDelete: ReferentialAction.Cascade);
//                    table.ForeignKey(
//                        name: "FK_APPLICATION_PHOTO_HISTORY_PACKAGE_PHOTO_HISTORY_PhotoPackageHistoryId",
//                        column: x => x.PhotoPackageHistoryId,
//                        principalTable: "PACKAGE_PHOTO_HISTORY",
//                        principalColumn: "ID",
//                        onDelete: ReferentialAction.Restrict);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "ATTACHMENT",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    PHOTO = table.Column<byte[]>(nullable: true),
//                    MINI_PHOTO = table.Column<byte[]>(nullable: true),
//                    UUID = table.Column<Guid>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_ATTACHMENT", x => x.ID);
//                });
//
//            migrationBuilder.CreateTable(
//                name: "APPLICATION_PHOTO",
//                columns: table => new
//                {
//                    ID = table.Column<long>(nullable: false),
//                    START_DATE = table.Column<DateTime>(nullable: false),
//                    UPDATE_DATE = table.Column<DateTime>(nullable: true),
//                    ERROR_TEXT = table.Column<string>(type: "NVARCHAR2", maxLength: 1000, nullable: true),
//                    ERROR_CODE = table.Column<string>(maxLength: 1000, nullable: true),
//                    LATITUDE = table.Column<double>(nullable: false),
//                    LONGITUDE = table.Column<double>(nullable: false),
//                    FOUND_DATE = table.Column<DateTime>(nullable: false),
//                    PHOTO_TYPE_ID = table.Column<int>(nullable: true),
//                    PARENT_PHOTO_TYPE_ID = table.Column<int>(nullable: true),
//                    IS_DELETED = table.Column<bool>(nullable: true),
//                    IS_SAVED = table.Column<bool>(nullable: true),
//                    ATTACHED_BY = table.Column<int>(nullable: false),
//                    PACKAGE_PHOTO_ID = table.Column<long>(nullable: false),
//                    ATTACHMENT_ID = table.Column<long>(nullable: false),
//                    AttachmentId1 = table.Column<long>(nullable: true)
//                },
//                constraints: table =>
//                {
//                    table.PrimaryKey("PK_APPLICATION_PHOTO", x => x.ID);
//                    table.UniqueConstraint("AK_APPLICATION_PHOTO_ATTACHMENT_ID", x => x.ATTACHMENT_ID);
//                    table.ForeignKey(
//                        name: "FK_APPLICATION_PHOTO_ATTACHMENT_AttachmentId1",
//                        column: x => x.AttachmentId1,
//                        principalTable: "ATTACHMENT",
//                        principalColumn: "ID",
//                        onDelete: ReferentialAction.Restrict);
//                    table.ForeignKey(
//                        name: "FK_APPLICATION_PHOTO_PACKAGE_PHOTO_PACKAGE_PHOTO_ID",
//                        column: x => x.PACKAGE_PHOTO_ID,
//                        principalTable: "PACKAGE_PHOTO",
//                        principalColumn: "ID",
//                        onDelete: ReferentialAction.Cascade);
//                });
//
//            migrationBuilder.CreateIndex(
//                name: "IX_APPLICATION_PHOTO_AttachmentId1",
//                table: "APPLICATION_PHOTO",
//                column: "AttachmentId1");
//
//            migrationBuilder.CreateIndex(
//                name: "IX_APPLICATION_PHOTO_PACKAGE_PHOTO_ID",
//                table: "APPLICATION_PHOTO",
//                column: "PACKAGE_PHOTO_ID");
//
//            migrationBuilder.CreateIndex(
//                name: "IX_APPLICATION_PHOTO_HISTORY_PACKAGE_HISTORY_ID",
//                table: "APPLICATION_PHOTO_HISTORY",
//                column: "PACKAGE_HISTORY_ID");
//
//            migrationBuilder.CreateIndex(
//                name: "IX_APPLICATION_PHOTO_HISTORY_PhotoPackageHistoryId",
//                table: "APPLICATION_PHOTO_HISTORY",
//                column: "PhotoPackageHistoryId");
//
//            migrationBuilder.AddForeignKey(
//                name: "FK_ATTACHMENT_APPLICATION_PHOTO_ID",
//                table: "ATTACHMENT",
//                column: "ID",
//                principalTable: "APPLICATION_PHOTO",
//                principalColumn: "ATTACHMENT_ID",
//                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
//            migrationBuilder.DropForeignKey(
//                name: "FK_APPLICATION_PHOTO_ATTACHMENT_AttachmentId1",
//                table: "APPLICATION_PHOTO");
//
//            migrationBuilder.DropTable(
//                name: "APPLICATION_PHOTO_HISTORY");
//
//            migrationBuilder.DropTable(
//                name: "CURRENT_PHOTO_REQUIRED");
//
//            migrationBuilder.DropTable(
//                name: "CURRENT_PHOTO_TYPE");
//
//            migrationBuilder.DropTable(
//                name: "PACKAGE_PHOTO_HISTORY");
//
//            migrationBuilder.DropTable(
//                name: "ATTACHMENT");
//
//            migrationBuilder.DropTable(
//                name: "APPLICATION_PHOTO");
//
//            migrationBuilder.DropTable(
//                name: "PACKAGE_PHOTO");
        }
    }
}
