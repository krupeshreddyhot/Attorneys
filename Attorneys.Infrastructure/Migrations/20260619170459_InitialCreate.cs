using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Attorneys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "case_types",
                columns: table => new
                {
                    CaseTypeId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_types", x => x.CaseTypeId);
                });

            migrationBuilder.CreateTable(
                name: "courts",
                columns: table => new
                {
                    CourtId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CourtName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CourtCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courts", x => x.CourtId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "cases",
                columns: table => new
                {
                    CaseNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CourtId = table.Column<string>(type: "character varying(20)", nullable: true),
                    CaseTypeId = table.Column<string>(type: "character varying(20)", nullable: true),
                    AppearingFor = table.Column<string>(type: "text", nullable: true),
                    ClientAddress = table.Column<string>(type: "text", nullable: true),
                    ClientPhone = table.Column<string>(type: "text", nullable: true),
                    SerialNo = table.Column<string>(type: "text", nullable: true),
                    DateOfFiling = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DateOfAppearance = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    OtherParty = table.Column<string>(type: "text", nullable: true),
                    CounselForOtherParty = table.Column<string>(type: "text", nullable: true),
                    Remarks = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cases", x => x.CaseNo);
                    table.ForeignKey(
                        name: "FK_cases_case_types_CaseTypeId",
                        column: x => x.CaseTypeId,
                        principalTable: "case_types",
                        principalColumn: "CaseTypeId");
                    table.ForeignKey(
                        name: "FK_cases_courts_CourtId",
                        column: x => x.CourtId,
                        principalTable: "courts",
                        principalColumn: "CourtId");
                });

            migrationBuilder.CreateTable(
                name: "case_accounts",
                columns: table => new
                {
                    CaseNo = table.Column<string>(type: "character varying(50)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_accounts", x => x.CaseNo);
                    table.ForeignKey(
                        name: "FK_case_accounts_cases_CaseNo",
                        column: x => x.CaseNo,
                        principalTable: "cases",
                        principalColumn: "CaseNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_details",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseNo = table.Column<string>(type: "character varying(50)", nullable: false),
                    CaseNoId = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<string>(type: "text", nullable: true),
                    PreviousDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NextDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Ia = table.Column<string>(type: "text", nullable: true),
                    IaStage = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_details", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_details_cases_CaseNo",
                        column: x => x.CaseNo,
                        principalTable: "cases",
                        principalColumn: "CaseNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "case_payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaseNo = table.Column<string>(type: "character varying(50)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_case_payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_case_payments_case_accounts_CaseNo",
                        column: x => x.CaseNo,
                        principalTable: "case_accounts",
                        principalColumn: "CaseNo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_case_details_CaseNo_CaseNoId",
                table: "case_details",
                columns: new[] { "CaseNo", "CaseNoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_case_payments_CaseNo",
                table: "case_payments",
                column: "CaseNo");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CaseTypeId",
                table: "cases",
                column: "CaseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_cases_CourtId",
                table: "cases",
                column: "CourtId");

            migrationBuilder.CreateIndex(
                name: "IX_users_UserName",
                table: "users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "case_details");

            migrationBuilder.DropTable(
                name: "case_payments");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "case_accounts");

            migrationBuilder.DropTable(
                name: "cases");

            migrationBuilder.DropTable(
                name: "case_types");

            migrationBuilder.DropTable(
                name: "courts");
        }
    }
}
