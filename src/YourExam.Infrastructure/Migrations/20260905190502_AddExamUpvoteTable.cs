using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExamUpvoteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamUpvotes",
                columns: table => new
                {
                    ExamId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamUpvotes", x => new { x.ExamId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ExamUpvotes_GeneratedExams_ExamId",
                        column: x => x.ExamId,
                        principalTable: "GeneratedExams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExamUpvotes_Profiles_UserId",
                        column: x => x.UserId,
                        principalTable: "Profiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamUpvotes_UserId",
                table: "ExamUpvotes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamUpvotes");
        }
    }
}
