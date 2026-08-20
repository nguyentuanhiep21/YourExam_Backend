using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace YourExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGeneratedExamQuestionTableWithDifficulty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "GeneratedExams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "GeneratedExams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "GeneratedExams",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalScore",
                table: "GeneratedExams",
                type: "numeric(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "GeneratedExams",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "GeneratedExamQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GeneratedExamId = table.Column<int>(type: "integer", nullable: false),
                    QuestionTemplateId = table.Column<int>(type: "integer", nullable: true),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    QuestionType = table.Column<int>(type: "integer", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    QuestionContent = table.Column<string>(type: "text", nullable: false),
                    MultipleChoiceOptions = table.Column<string>(type: "jsonb", nullable: true),
                    CorrectAnswer = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Score = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedExamQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedExamQuestions_GeneratedExams_GeneratedExamId",
                        column: x => x.GeneratedExamId,
                        principalTable: "GeneratedExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GeneratedExamQuestions_QuestionTemplates_QuestionTemplateId",
                        column: x => x.QuestionTemplateId,
                        principalTable: "QuestionTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedExamQuestions_GeneratedExamId_OrderIndex",
                table: "GeneratedExamQuestions",
                columns: new[] { "GeneratedExamId", "OrderIndex" });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedExamQuestions_QuestionTemplateId",
                table: "GeneratedExamQuestions",
                column: "QuestionTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedExamQuestions");

            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "GeneratedExams");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "GeneratedExams");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "GeneratedExams");

            migrationBuilder.DropColumn(
                name: "TotalScore",
                table: "GeneratedExams");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "GeneratedExams");
        }
    }
}
