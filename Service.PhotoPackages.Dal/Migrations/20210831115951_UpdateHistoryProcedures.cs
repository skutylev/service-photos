using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class UpdateHistoryProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE ADD_PHOTO_HISTORY_PR (
                                    NEW_ID IN NVARCHAR2,
                                    NEW_CONTENT_ID IN NVARCHAR2,
                                    NEW_THUMBNAIL_CONTENT_ID IN NVARCHAR2,
                                    NEW_ERROR_CODE IN NVARCHAR2,
                                    NEW_ERROR_MESSAGE IN NVARCHAR2,
                                    NEW_STATUS_CODE IN NVARCHAR2
                                 ) AS
                                 NEW_START_DATE DATE; 
                                 BEGIN 
                                    NEW_START_DATE := SYSDATE-3/24;
                                    INSERT INTO PH_PHOTO_HISTORY (
                                          PHOTO_ID,
                                          CONTENT_ID,
                                          THUMBNAIL_CONTENT_ID,
                                          ERROR_CODE,
                                          ERROR_MESSAGE,
                                          START_DATE,
                                          STATUS_CODE
                                       ) VALUES (
                                          NEW_ID,
                                          NEW_CONTENT_ID,
                                          NEW_THUMBNAIL_CONTENT_ID,
                                          NEW_ERROR_CODE,
                                          NEW_ERROR_MESSAGE,
                                          NEW_START_DATE,
                                          NEW_STATUS_CODE
                                       );
                                 END ADD_PHOTO_HISTORY_PR;");
            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER ADD_PHOTO_HISTORY_TRG  
                                    AFTER INSERT OR UPDATE OF
                                       CONTENT_ID,
                                       THUMBNAIL_CONTENT_ID,
                                       ERROR_CODE,
                                       ERROR_MESSAGE,
                                       STATUS_CODE
                                    ON PH_PHOTO FOR EACH ROW
                                    BEGIN
                                       ADD_PHOTO_HISTORY_PR(
                                          :NEW.ID,
                                          :NEW.CONTENT_ID,
                                          :NEW.THUMBNAIL_CONTENT_ID,
                                          :NEW.ERROR_CODE,
                                          :NEW.ERROR_MESSAGE,
                                          :NEW.STATUS_CODE
                                       );
                                    END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(@"DROP TRIGGER ADD_PHOTO_HISTORY_TRG");
           migrationBuilder.Sql(@"DROP PROCEDURE ADD_PHOTO_HISTORY_PR");
        }
    }
}
