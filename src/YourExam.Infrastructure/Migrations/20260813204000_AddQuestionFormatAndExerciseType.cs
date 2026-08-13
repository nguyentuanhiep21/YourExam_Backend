using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionFormatAndExerciseType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionFormat",
                table: "QuestionTemplates",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExerciseType",
                table: "BlueprintRules",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionFormat",
                table: "QuestionTemplates");

            migrationBuilder.DropColumn(
                name: "ExerciseType",
                table: "BlueprintRules");
        }
    }
}
