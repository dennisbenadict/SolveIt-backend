using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsBlockedToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_organizers_organizer_id",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                table: "participants",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "is_blocked",
                table: "organizers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_blocked",
                table: "participants");

            migrationBuilder.DropColumn(
                name: "is_blocked",
                table: "organizers");

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_organizers_organizer_id",
                table: "refresh_tokens",
                column: "organizer_id",
                principalTable: "organizers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
