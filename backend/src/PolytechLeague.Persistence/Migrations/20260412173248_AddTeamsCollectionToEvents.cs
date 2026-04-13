using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamsCollectionToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "VolleyballEventId",
                table: "volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FutsalEventId",
                table: "futsal_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BeachVolleyballEventId",
                table: "beach_volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Basketball3x3EventId",
                table: "basketball3x3_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_teams_VolleyballEventId",
                table: "volleyball_teams",
                column: "VolleyballEventId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_FutsalEventId",
                table: "futsal_teams",
                column: "FutsalEventId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_teams_BeachVolleyballEventId",
                table: "beach_volleyball_teams",
                column: "BeachVolleyballEventId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_teams_Basketball3x3EventId",
                table: "basketball3x3_teams",
                column: "Basketball3x3EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_teams_basketball3x3_events_Basketball3x3Event~",
                table: "basketball3x3_teams",
                column: "Basketball3x3EventId",
                principalTable: "basketball3x3_events",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_teams_beach_volleyball_events_BeachVolleyb~",
                table: "beach_volleyball_teams",
                column: "BeachVolleyballEventId",
                principalTable: "beach_volleyball_events",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_teams_futsal_events_FutsalEventId",
                table: "futsal_teams",
                column: "FutsalEventId",
                principalTable: "futsal_events",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_teams_volleyball_events_VolleyballEventId",
                table: "volleyball_teams",
                column: "VolleyballEventId",
                principalTable: "volleyball_events",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_teams_basketball3x3_events_Basketball3x3Event~",
                table: "basketball3x3_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_teams_beach_volleyball_events_BeachVolleyb~",
                table: "beach_volleyball_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_teams_futsal_events_FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_teams_volleyball_events_VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_teams_VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_teams_BeachVolleyballEventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_teams_Basketball3x3EventId",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "BeachVolleyballEventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "Basketball3x3EventId",
                table: "basketball3x3_teams");
        }
    }
}
