using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SolveIt.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrialUsageTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trial_usages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organizer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_fingerprint = table.Column<string>(type: "text", nullable: true),
                    ip_address = table.Column<string>(type: "varchar(64)", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trial_usages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trial_usages_device_fingerprint",
                table: "trial_usages",
                column: "device_fingerprint");

            migrationBuilder.CreateIndex(
                name: "IX_trial_usages_ip_address",
                table: "trial_usages",
                column: "ip_address");

            migrationBuilder.CreateIndex(
                name: "IX_trial_usages_organizer_id",
                table: "trial_usages",
                column: "organizer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trial_usages");
        }
    }
}
