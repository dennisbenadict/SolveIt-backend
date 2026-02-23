using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenLineage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReplacedByTokenId",
                table: "refresh_tokens",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAtUtc",
                table: "refresh_tokens",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplacedByTokenId",
                table: "refresh_tokens");

            migrationBuilder.DropColumn(
                name: "RevokedAtUtc",
                table: "refresh_tokens");
        }
    }
}
