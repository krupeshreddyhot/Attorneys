using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Attorneys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMultiTenancy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_accounts_cases_CaseNo",
                table: "case_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_case_details_cases_CaseNo",
                table: "case_details");

            migrationBuilder.DropForeignKey(
                name: "FK_case_documents_cases_CaseNo",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_case_payments_case_accounts_CaseNo",
                table: "case_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_case_types_CaseTypeId",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_courts_CourtId",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_users_UserName",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courts",
                table: "courts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cases",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_CaseTypeId",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_CourtId",
                table: "cases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_types",
                table: "case_types");

            migrationBuilder.DropIndex(
                name: "IX_case_payments_CaseNo",
                table: "case_payments");

            migrationBuilder.DropIndex(
                name: "IX_case_documents_CaseNo",
                table: "case_documents");

            migrationBuilder.DropIndex(
                name: "IX_case_details_CaseNo_CaseNoId",
                table: "case_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_accounts",
                table: "case_accounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "users",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "courts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "courts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfFiling",
                table: "cases",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAppearance",
                table: "cases",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "cases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "case_types",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "case_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "case_payments",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "case_payments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "case_payments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAtUtc",
                table: "case_documents",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "case_documents",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "case_documents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PreviousDate",
                table: "case_details",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextDate",
                table: "case_details",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "case_details",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "case_details",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "case_accounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "case_accounts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_courts",
                table: "courts",
                columns: new[] { "TenantId", "CourtId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_cases",
                table: "cases",
                columns: new[] { "TenantId", "CaseNo" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_types",
                table: "case_types",
                columns: new[] { "TenantId", "CaseTypeId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_accounts",
                table: "case_accounts",
                columns: new[] { "TenantId", "CaseNo" });

            migrationBuilder.CreateTable(
                name: "tenants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tenants", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_TenantId_UserName",
                table: "users",
                columns: new[] { "TenantId", "UserName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cases_TenantId_CaseTypeId",
                table: "cases",
                columns: new[] { "TenantId", "CaseTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_cases_TenantId_CourtId",
                table: "cases",
                columns: new[] { "TenantId", "CourtId" });

            migrationBuilder.CreateIndex(
                name: "IX_case_payments_TenantId_CaseNo",
                table: "case_payments",
                columns: new[] { "TenantId", "CaseNo" });

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_TenantId_CaseNo",
                table: "case_documents",
                columns: new[] { "TenantId", "CaseNo" });

            migrationBuilder.CreateIndex(
                name: "IX_case_details_TenantId_CaseNo_CaseNoId",
                table: "case_details",
                columns: new[] { "TenantId", "CaseNo", "CaseNoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tenants_Code",
                table: "tenants",
                column: "Code",
                unique: true);

            migrationBuilder.InsertData(
                table: "tenants",
                columns: new[] { "Id", "Name", "Code", "IsActive", "CreatedAtUtc" },
                values: new object[] { 1, "Demo Law Firm", "DEMO", true, new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.Sql("""
                UPDATE courts SET "TenantId" = 1;
                UPDATE case_types SET "TenantId" = 1;
                UPDATE cases SET "TenantId" = 1;
                UPDATE case_details SET "TenantId" = 1;
                UPDATE case_accounts SET "TenantId" = 1;
                UPDATE case_payments SET "TenantId" = 1;
                UPDATE case_documents SET "TenantId" = 1;
                UPDATE users SET "TenantId" = 1;
                SELECT setval(pg_get_serial_sequence('tenants', 'Id'), (SELECT COALESCE(MAX("Id"), 1) FROM tenants));
                """);

            migrationBuilder.AddForeignKey(
                name: "FK_case_accounts_cases_TenantId_CaseNo",
                table: "case_accounts",
                columns: new[] { "TenantId", "CaseNo" },
                principalTable: "cases",
                principalColumns: new[] { "TenantId", "CaseNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_details_cases_TenantId_CaseNo",
                table: "case_details",
                columns: new[] { "TenantId", "CaseNo" },
                principalTable: "cases",
                principalColumns: new[] { "TenantId", "CaseNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_documents_cases_TenantId_CaseNo",
                table: "case_documents",
                columns: new[] { "TenantId", "CaseNo" },
                principalTable: "cases",
                principalColumns: new[] { "TenantId", "CaseNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_payments_case_accounts_TenantId_CaseNo",
                table: "case_payments",
                columns: new[] { "TenantId", "CaseNo" },
                principalTable: "case_accounts",
                principalColumns: new[] { "TenantId", "CaseNo" },
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_case_types_TenantId_CaseTypeId",
                table: "cases",
                columns: new[] { "TenantId", "CaseTypeId" },
                principalTable: "case_types",
                principalColumns: new[] { "TenantId", "CaseTypeId" });

            migrationBuilder.AddForeignKey(
                name: "FK_cases_courts_TenantId_CourtId",
                table: "cases",
                columns: new[] { "TenantId", "CourtId" },
                principalTable: "courts",
                principalColumns: new[] { "TenantId", "CourtId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_case_accounts_cases_TenantId_CaseNo",
                table: "case_accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_case_details_cases_TenantId_CaseNo",
                table: "case_details");

            migrationBuilder.DropForeignKey(
                name: "FK_case_documents_cases_TenantId_CaseNo",
                table: "case_documents");

            migrationBuilder.DropForeignKey(
                name: "FK_case_payments_case_accounts_TenantId_CaseNo",
                table: "case_payments");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_case_types_TenantId_CaseTypeId",
                table: "cases");

            migrationBuilder.DropForeignKey(
                name: "FK_cases_courts_TenantId_CourtId",
                table: "cases");

            migrationBuilder.DropTable(
                name: "tenants");

            migrationBuilder.DropIndex(
                name: "IX_users_TenantId_UserName",
                table: "users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_courts",
                table: "courts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cases",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_TenantId_CaseTypeId",
                table: "cases");

            migrationBuilder.DropIndex(
                name: "IX_cases_TenantId_CourtId",
                table: "cases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_types",
                table: "case_types");

            migrationBuilder.DropIndex(
                name: "IX_case_payments_TenantId_CaseNo",
                table: "case_payments");

            migrationBuilder.DropIndex(
                name: "IX_case_documents_TenantId_CaseNo",
                table: "case_documents");

            migrationBuilder.DropIndex(
                name: "IX_case_details_TenantId_CaseNo_CaseNoId",
                table: "case_details");

            migrationBuilder.DropPrimaryKey(
                name: "PK_case_accounts",
                table: "case_accounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "courts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "courts");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "cases");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "case_types");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "case_types");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "case_payments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "case_payments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "case_documents");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "case_documents");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "case_details");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "case_details");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "case_accounts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "case_accounts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfFiling",
                table: "cases",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfAppearance",
                table: "cases",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidDate",
                table: "case_payments",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UploadedAtUtc",
                table: "case_documents",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PreviousDate",
                table: "case_details",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NextDate",
                table: "case_details",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_courts",
                table: "courts",
                column: "CourtId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_cases",
                table: "cases",
                column: "CaseNo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_types",
                table: "case_types",
                column: "CaseTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_case_accounts",
                table: "case_accounts",
                column: "CaseNo");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_cases_CaseTypeId",
                table: "cases",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CourtId",
                table: "cases",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_case_payments_CaseNo",
                table: "case_payments",
                column: "CaseNo");

            migrationBuilder.CreateIndex(
                name: "IX_case_documents_CaseNo",
                table: "case_documents",
                column: "CaseNo");

            migrationBuilder.CreateIndex(
                name: "IX_case_details_CaseNo_CaseNoId",
                table: "case_details",
                columns: new[] { "CaseNo", "CaseNoId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_case_accounts_cases_CaseNo",
                table: "case_accounts",
                column: "CaseNo",
                principalTable: "cases",
                principalColumn: "CaseNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_details_cases_CaseNo",
                table: "case_details",
                column: "CaseNo",
                principalTable: "cases",
                principalColumn: "CaseNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_documents_cases_CaseNo",
                table: "case_documents",
                column: "CaseNo",
                principalTable: "cases",
                principalColumn: "CaseNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_case_payments_case_accounts_CaseNo",
                table: "case_payments",
                column: "CaseNo",
                principalTable: "case_accounts",
                principalColumn: "CaseNo",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_cases_case_types_CaseTypeId",
                table: "cases",
                column: "CaseTypeId",
                principalTable: "case_types",
                principalColumn: "CaseTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_cases_courts_CourtId",
                table: "cases",
                column: "CourtId",
                principalTable: "courts",
                principalColumn: "CourtId");
        }
    }
}
