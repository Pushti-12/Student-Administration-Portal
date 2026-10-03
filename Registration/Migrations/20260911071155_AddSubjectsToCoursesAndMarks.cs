using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Registration.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectsToCoursesAndMarks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "marks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubjectName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_subjects_courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_marks_SubjectId",
                table: "marks",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_subjects_CourseId_SubjectName",
                table: "subjects",
                columns: new[] { "CourseId", "SubjectName" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_marks_subjects_SubjectId",
                table: "marks",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_marks_subjects_SubjectId",
                table: "marks");

            migrationBuilder.DropTable(
                name: "subjects");

            migrationBuilder.DropIndex(
                name: "IX_marks_SubjectId",
                table: "marks");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "marks");
        }
    }
}
