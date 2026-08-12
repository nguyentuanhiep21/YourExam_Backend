using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YourExam.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    School = table.Column<string>(type: "text", nullable: true),
                    SubjectsTaught = table.Column<string>(type: "text", nullable: true),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profiles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Votes_UserId",
                table: "Votes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedExams_AuthorId",
                table: "GeneratedExams",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamBlueprints_AuthorId",
                table: "ExamBlueprints",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamBlueprints_Profiles_AuthorId",
                table: "ExamBlueprints",
                column: "AuthorId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GeneratedExams_Profiles_AuthorId",
                table: "GeneratedExams",
                column: "AuthorId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Votes_Profiles_UserId",
                table: "Votes",
                column: "UserId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamBlueprints_Profiles_AuthorId",
                table: "ExamBlueprints");

            migrationBuilder.DropForeignKey(
                name: "FK_GeneratedExams_Profiles_AuthorId",
                table: "GeneratedExams");

            migrationBuilder.DropForeignKey(
                name: "FK_Votes_Profiles_UserId",
                table: "Votes");

            migrationBuilder.DropTable(
                name: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Votes_UserId",
                table: "Votes");

            migrationBuilder.DropIndex(
                name: "IX_GeneratedExams_AuthorId",
                table: "GeneratedExams");

            migrationBuilder.DropIndex(
                name: "IX_ExamBlueprints_AuthorId",
                table: "ExamBlueprints");
        }
    }
}
