using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UNIFY_CHECKERS_PARTICIPANTS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_checkers_events_CheckersEventId",
                table: "checkers_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_users_UserId",
                table: "checkers_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants");

            migrationBuilder.RenameColumn(
                name: "points_earned",
                table: "checkers_event_participants",
                newName: "points");

            migrationBuilder.RenameColumn(
                name: "CheckersEventId",
                table: "checkers_event_participants",
                newName: "TournamentId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "checkers_event_participants",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_event_id",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_temp");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_user_id",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_event_id");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_temp",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_user_id");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "checkers_event_participants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants",
                columns: new[] { "TournamentId", "points" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants",
                columns: new[] { "TournamentId", "PlayerId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_checkers_events_TournamentId",
                table: "checkers_event_participants",
                column: "TournamentId",
                principalTable: "checkers_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_checkers_players_PlayerId",
                table: "checkers_event_participants",
                column: "PlayerId",
                principalTable: "checkers_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_checkers_events_TournamentId",
                table: "checkers_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_checkers_players_PlayerId",
                table: "checkers_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "checkers_event_participants");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "checkers_event_participants",
                newName: "points_earned");

            migrationBuilder.RenameColumn(
                name: "TournamentId",
                table: "checkers_event_participants",
                newName: "CheckersEventId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "checkers_event_participants",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_event_id",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_temp");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_user_id",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_event_id");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_event_participants_temp",
                table: "checkers_event_participants",
                newName: "IX_checkers_event_participants_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants",
                columns: new[] { "CheckersEventId", "points_earned" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants",
                columns: new[] { "CheckersEventId", "UserId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_checkers_events_CheckersEventId",
                table: "checkers_event_participants",
                column: "CheckersEventId",
                principalTable: "checkers_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_users_UserId",
                table: "checkers_event_participants",
                column: "UserId",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
