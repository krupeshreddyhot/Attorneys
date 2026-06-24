using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attorneys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCaseDocumentAIAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_case_document_ai_analysis_FileId",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "ImportantDates",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "KeyPoints",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "NextActions",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "Parties",
                table: "case_document_ai_analysis");

            migrationBuilder.Sql("""
                ALTER TABLE case_document_ai_analysis
                ALTER COLUMN "Status" TYPE integer
                USING (
                    CASE "Status"
                        WHEN 'Pending' THEN 1
                        WHEN 'Processing' THEN 2
                        WHEN 'Completed' THEN 3
                        WHEN 'Failed' THEN 4
                        ELSE 1
                    END
                );
                """);

            migrationBuilder.AddColumn<string>(
                name: "AIModel",
                table: "case_document_ai_analysis",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EstimatedCost",
                table: "case_document_ai_analysis",
                type: "numeric(18,6)",
                precision: 18,
                scale: 6,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtractedTextStorageKey",
                table: "case_document_ai_analysis",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImportantDatesJson",
                table: "case_document_ai_analysis",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InputTokens",
                table: "case_document_ai_analysis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyPointsJson",
                table: "case_document_ai_analysis",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextActionsJson",
                table: "case_document_ai_analysis",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OutputTokens",
                table: "case_document_ai_analysis",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartiesJson",
                table: "case_document_ai_analysis",
                type: "jsonb",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_case_document_ai_analysis_FileId",
                table: "case_document_ai_analysis",
                column: "FileId",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_case_document_ai_analysis_FileId",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "AIModel",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "EstimatedCost",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "ExtractedTextStorageKey",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "ImportantDatesJson",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "InputTokens",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "KeyPointsJson",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "NextActionsJson",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "OutputTokens",
                table: "case_document_ai_analysis");

            migrationBuilder.DropColumn(
                name: "PartiesJson",
                table: "case_document_ai_analysis");

            migrationBuilder.Sql("""
                ALTER TABLE case_document_ai_analysis
                ALTER COLUMN "Status" TYPE character varying(50)
                USING (
                    CASE "Status"
                        WHEN 1 THEN 'Pending'
                        WHEN 2 THEN 'Processing'
                        WHEN 3 THEN 'Completed'
                        WHEN 4 THEN 'Failed'
                        ELSE 'Pending'
                    END
                );
                """);

            migrationBuilder.AddColumn<string>(
                name: "ImportantDates",
                table: "case_document_ai_analysis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyPoints",
                table: "case_document_ai_analysis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NextActions",
                table: "case_document_ai_analysis",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Parties",
                table: "case_document_ai_analysis",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_case_document_ai_analysis_FileId",
                table: "case_document_ai_analysis",
                column: "FileId");
        }
    }
}
