using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attorneys.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPromptVersionToCaseDocumentAIAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PromptVersion",
                table: "case_document_ai_analysis",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromptVersion",
                table: "case_document_ai_analysis");
        }
    }
}
