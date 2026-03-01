using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTournamentAggregate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organizer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    start_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    is_free_trial = table.Column<bool>(type: "boolean", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    row_version = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments", x => x.id);
                    table.ForeignKey(
                        name: "FK_tournaments_organizers_organizer_id",
                        column: x => x.organizer_id,
                        principalTable: "organizers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_organizer_created",
                table: "tournaments",
                columns: new[] { "organizer_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_organizer_id",
                table: "tournaments",
                column: "organizer_id");

            migrationBuilder.CreateIndex(
                name: "IX_tournaments_status",
                table: "tournaments",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tournaments");
        }
    }
}
