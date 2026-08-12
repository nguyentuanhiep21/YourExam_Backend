using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorQuestionTypeToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestionType",
                table: "QuestionTemplates",
                newName: "ExerciseType");

            migrationBuilder.RenameColumn(
                name: "QuestionType",
                table: "BlueprintRules",
                newName: "QuestionFormat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExerciseType",
                table: "QuestionTemplates",
                newName: "QuestionType");

            migrationBuilder.RenameColumn(
                name: "QuestionFormat",
                table: "BlueprintRules",
                newName: "QuestionType");
        }
    }
}
