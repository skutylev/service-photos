﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace Service.PhotoPackages.Dal.Migrations
{
    public partial class UpdateHistoryProcedures2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                       migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE ADD_PHOTO_PACKAGE_HISTORY_PR (
                                       NEW_PHOTO_PACKAGE_ID IN NVARCHAR2,
                                       NEW_STATUS_CODE IN NVARCHAR2,
                                       NEW_AUTHOR_SAP_NUMBER IN NUMBER,
                                       NEW_VERIFIER_SAP_NUMBER IN NUMBER,
                                       NEW_LATITUDE IN BINARY_DOUBLE,
                                       NEW_LONGITUDE IN BINARY_DOUBLE,
                                       NEW_ERROR_CODE IN NVARCHAR2,
                                       NEW_ERROR_MESSAGE IN NVARCHAR2,
                                       NEW_SCAN_CHECKER_REQUEST_ID NVARCHAR2
                                    ) AS 
                                    NEW_START_DATE DATE;
                                    BEGIN 
                                       NEW_START_DATE := SYSDATE-3/24;
                                       INSERT INTO PH_PHOTO_PACKAGE_HISTORY (
                                             PHOTO_PACKAGE_ID,
                                             STATUS_CODE,
                                             AUTHOR_SAP_NUMBER,
                                             VERIFIER_SAP_NUMBER,
                                             LATITUDE,
                                             LONGITUDE,
                                             ERROR_CODE,
                                             ERROR_MESSAGE,
                                             START_DATE,
                                             SCAN_CHECKER_REQUEST_ID
                                          ) VALUES (
                                             NEW_PHOTO_PACKAGE_ID,
                                             NEW_STATUS_CODE,
                                             NEW_AUTHOR_SAP_NUMBER,
                                             NEW_VERIFIER_SAP_NUMBER,
                                             NEW_LATITUDE,
                                             NEW_LONGITUDE,
                                             NEW_ERROR_CODE,
                                             NEW_ERROR_MESSAGE,
                                             NEW_START_DATE,
                                             NEW_SCAN_CHECKER_REQUEST_ID
                                          );
                                    END ADD_PHOTO_PACKAGE_HISTORY_PR;");
            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER ADD_PHOTO_PACKAGE_HISTORY_TRG
                                    AFTER INSERT OR UPDATE OF
                                       STATUS_CODE,
                                       AUTHOR_SAP_NUMBER,
                                       VERIFIER_SAP_NUMBER,
                                       LATITUDE,
                                       LONGITUDE,
                                       ERROR_CODE,
                                       ERROR_MESSAGE,
                                       SCAN_CHECKER_REQUEST_ID
                                    ON PH_PHOTO_PACKAGE FOR EACH ROW
                                    BEGIN
                                       ADD_PHOTO_PACKAGE_HISTORY_PR(
                                          :NEW.ID,
                                          :NEW.STATUS_CODE,
                                          :NEW.AUTHOR_SAP_NUMBER,
                                          :NEW.VERIFIER_SAP_NUMBER,
                                          :NEW.LATITUDE,
                                          :NEW.LONGITUDE,
                                          :NEW.ERROR_CODE,
                                          :NEW.ERROR_MESSAGE,
                                          :NEW.SCAN_CHECKER_REQUEST_ID
                                          );
                                    END;");
            
            migrationBuilder.Sql(@"CREATE OR REPLACE PROCEDURE ADD_PHOTO_HISTORY_PR (
                                    NEW_ID IN NVARCHAR2,
                                    NEW_CONTENT_ID IN NVARCHAR2,
                                    NEW_THUMBNAIL_CONTENT_ID IN NVARCHAR2,
                                    NEW_ERROR_CODE IN NVARCHAR2,
                                    NEW_ERROR_MESSAGE IN NVARCHAR2,
                                    NEW_STATUS_CODE IN NVARCHAR2,
                                    NEW_SCAN_SERVER_DOCUMENT_ID IN NVARCHAR2,
                                    NEW_SCAN_SERVER_PAGE_ID IN NVARCHAR2,
                                    NEW_SCAN_SERVER_PAGE_NUMBER IN NVARCHAR2,
                                    NEW_TRANZ_WARE_SCAN_ID IN NVARCHAR2
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
                                          STATUS_CODE,
                                          SCAN_SERVER_DOCUMENT_ID,
                                          SCAN_SERVER_PAGE_ID,
                                          SCAN_SERVER_PAGE_NUMBER,
                                          TRANZ_WARE_SCAN_ID
                                       ) VALUES (
                                          NEW_ID,
                                          NEW_CONTENT_ID,
                                          NEW_THUMBNAIL_CONTENT_ID,
                                          NEW_ERROR_CODE,
                                          NEW_ERROR_MESSAGE,
                                          NEW_START_DATE,
                                          NEW_STATUS_CODE,
                                          NEW_SCAN_SERVER_DOCUMENT_ID,
                                          NEW_SCAN_SERVER_PAGE_ID,
                                          NEW_SCAN_SERVER_PAGE_NUMBER,
                                          NEW_TRANZ_WARE_SCAN_ID
                                       );
                                 END ADD_PHOTO_HISTORY_PR;");
            migrationBuilder.Sql(@"CREATE OR REPLACE TRIGGER ADD_PHOTO_HISTORY_TRG  
                                    AFTER INSERT OR UPDATE OF
                                       CONTENT_ID,
                                       THUMBNAIL_CONTENT_ID,
                                       ERROR_CODE,
                                       ERROR_MESSAGE,
                                       STATUS_CODE,
                                       SCAN_SERVER_DOCUMENT_ID,
                                       SCAN_SERVER_PAGE_ID,
                                       SCAN_SERVER_PAGE_NUMBER,
                                       TRANZ_WARE_SCAN_ID
                                    ON PH_PHOTO FOR EACH ROW
                                    BEGIN
                                       ADD_PHOTO_HISTORY_PR(
                                          :NEW.ID,
                                          :NEW.CONTENT_ID,
                                          :NEW.THUMBNAIL_CONTENT_ID,
                                          :NEW.ERROR_CODE,
                                          :NEW.ERROR_MESSAGE,
                                          :NEW.STATUS_CODE,
                                          :NEW.SCAN_SERVER_DOCUMENT_ID,
                                          :NEW.SCAN_SERVER_PAGE_ID,
                                          :NEW.SCAN_SERVER_PAGE_NUMBER,
                                          :NEW.TRANZ_WARE_SCAN_ID
                                       );
                                    END;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.Sql(@"DROP TRIGGER ADD_PHOTO_PACKAGE_HISTORY_TRG");
           migrationBuilder.Sql(@"DROP PROCEDURE ADD_PHOTO_PACKAGE_HISTORY_PR");
           migrationBuilder.Sql(@"DROP TRIGGER ADD_PHOTO_HISTORY_TRG");
           migrationBuilder.Sql(@"DROP PROCEDURE ADD_PHOTO_HISTORY_PR");
        }
    }
}
