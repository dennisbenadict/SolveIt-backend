using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerFreeTrialFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "created_at_utc",
                table: "trial_usages",
                newName: "used_at_utc");

            migrationBuilder.AddColumn<DateTime>(
                name: "free_trial_used_at_utc",
                table: "organizers",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "free_trial_used_at_utc",
                table: "organizers");

            migrationBuilder.RenameColumn(
                name: "used_at_utc",
                table: "trial_usages",
                newName: "created_at_utc");
        }
    }
}
