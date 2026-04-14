using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RENAME_BADMINTON_TOURNAMENT_ID_TO_EVENT_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_event_participants_badminton_events_TournamentId",
                table: "badminton_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_events_TournamentId",
                table: "badminton_matches");

            migrationBuilder.RenameColumn(
                name: "TournamentId",
                table: "badminton_matches",
                newName: "EventId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_TournamentId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_EventId");

            migrationBuilder.RenameColumn(
                name: "TournamentId",
                table: "badminton_event_participants",
                newName: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_badminton_events_EventId",
                table: "badminton_event_participants",
                column: "EventId",
                principalTable: "badminton_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_events_EventId",
                table: "badminton_matches",
                column: "EventId",
                principalTable: "badminton_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_event_participants_badminton_events_EventId",
                table: "badminton_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_events_EventId",
                table: "badminton_matches");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "badminton_matches",
                newName: "TournamentId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_EventId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_TournamentId");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "badminton_event_participants",
                newName: "TournamentId");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_badminton_events_TournamentId",
                table: "badminton_event_participants",
                column: "TournamentId",
                principalTable: "badminton_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_events_TournamentId",
                table: "badminton_matches",
                column: "TournamentId",
                principalTable: "badminton_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
