using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLanguageToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        ALTER TABLE submissions
        ALTER COLUMN language TYPE integer
        USING CASE
            WHEN lower(language) = 'python' THEN 1
            WHEN lower(language) = 'csharp' THEN 2
            WHEN lower(language) = 'javascript' THEN 3
            WHEN lower(language) = 'go' THEN 4
            WHEN lower(language) = 'dart' THEN 5
            ELSE 1
        END
    ");

            migrationBuilder.AddForeignKey(
                name: "FK_submissions_tournament_problems_problem_id",
                table: "submissions",
                column: "problem_id",
                principalTable: "tournament_problems",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_submissions_tournament_problems_problem_id",
                table: "submissions");

            migrationBuilder.Sql(@"
        ALTER TABLE submissions
        ALTER COLUMN language TYPE varchar(50)
        USING CASE
            WHEN language = 1 THEN 'python'
            WHEN language = 2 THEN 'csharp'
            WHEN language = 3 THEN 'javascript'
            WHEN language = 4 THEN 'go'
            WHEN language = 5 THEN 'dart'
        END
    ");
        }
    }
}
