using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenLineNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RevokedAtUtc",
                table: "refresh_tokens",
                newName: "revoked_at_utc");

            migrationBuilder.RenameColumn(
                name: "ReplacedByTokenId",
                table: "refresh_tokens",
                newName: "replaced_by_token_id");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_expires_at_utc",
                table: "refresh_tokens",
                column: "expires_at_utc");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_replaced_by_token_id",
                table: "refresh_tokens",
                column: "replaced_by_token_id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_refresh_tokens_refresh_tokens_replaced_by_token_id",
                table: "refresh_tokens",
                column: "replaced_by_token_id",
                principalTable: "refresh_tokens",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_refresh_tokens_refresh_tokens_replaced_by_token_id",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_expires_at_utc",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_replaced_by_token_id",
                table: "refresh_tokens");

            migrationBuilder.RenameColumn(
                name: "revoked_at_utc",
                table: "refresh_tokens",
                newName: "RevokedAtUtc");

            migrationBuilder.RenameColumn(
                name: "replaced_by_token_id",
                table: "refresh_tokens",
                newName: "ReplacedByTokenId");
        }
    }
}
