using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FUTSAL_RENAME_ENTIIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FutsalEvents_events_id",
                table: "FutsalEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalMatches_FutsalEvents_EventId",
                table: "FutsalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_AwayTeamId",
                table: "FutsalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_HomeTeamId",
                table: "FutsalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_WinnerTeamId",
                table: "FutsalMatches");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalPlayers_FutsalTeams_TeamId",
                table: "FutsalPlayers");

            migrationBuilder.DropForeignKey(
                name: "FK_FutsalPlayers_persons_id",
                table: "FutsalPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FutsalTeams",
                table: "FutsalTeams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FutsalPlayers",
                table: "FutsalPlayers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FutsalMatches",
                table: "FutsalMatches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FutsalEvents",
                table: "FutsalEvents");

            migrationBuilder.RenameTable(
                name: "FutsalTeams",
                newName: "futsal_teams");

            migrationBuilder.RenameTable(
                name: "FutsalPlayers",
                newName: "futsal_players");

            migrationBuilder.RenameTable(
                name: "FutsalMatches",
                newName: "futsal_matches");

            migrationBuilder.RenameTable(
                name: "FutsalEvents",
                newName: "futsal_events");

            migrationBuilder.RenameIndex(
                name: "IX_FutsalPlayers_TeamId",
                table: "futsal_players",
                newName: "IX_futsal_players_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_FutsalMatches_WinnerTeamId",
                table: "futsal_matches",
                newName: "IX_futsal_matches_WinnerTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_FutsalMatches_HomeTeamId",
                table: "futsal_matches",
                newName: "IX_futsal_matches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_FutsalMatches_EventId",
                table: "futsal_matches",
                newName: "IX_futsal_matches_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_FutsalMatches_AwayTeamId",
                table: "futsal_matches",
                newName: "IX_futsal_matches_AwayTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_futsal_teams",
                table: "futsal_teams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_futsal_players",
                table: "futsal_players",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_futsal_matches",
                table: "futsal_matches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_futsal_events",
                table: "futsal_events",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_events_id",
                table: "futsal_events",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_futsal_events_EventId",
                table: "futsal_matches",
                column: "EventId",
                principalTable: "futsal_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_futsal_teams_AwayTeamId",
                table: "futsal_matches",
                column: "AwayTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_futsal_teams_HomeTeamId",
                table: "futsal_matches",
                column: "HomeTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_futsal_teams_WinnerTeamId",
                table: "futsal_matches",
                column: "WinnerTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_players_futsal_teams_TeamId",
                table: "futsal_players",
                column: "TeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_players_persons_id",
                table: "futsal_players",
                column: "id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_events_id",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_futsal_events_EventId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_futsal_teams_AwayTeamId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_futsal_teams_HomeTeamId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_futsal_teams_WinnerTeamId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_players_futsal_teams_TeamId",
                table: "futsal_players");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_players_persons_id",
                table: "futsal_players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_futsal_teams",
                table: "futsal_teams");

            migrationBuilder.DropPrimaryKey(
                name: "PK_futsal_players",
                table: "futsal_players");

            migrationBuilder.DropPrimaryKey(
                name: "PK_futsal_matches",
                table: "futsal_matches");

            migrationBuilder.DropPrimaryKey(
                name: "PK_futsal_events",
                table: "futsal_events");

            migrationBuilder.RenameTable(
                name: "futsal_teams",
                newName: "FutsalTeams");

            migrationBuilder.RenameTable(
                name: "futsal_players",
                newName: "FutsalPlayers");

            migrationBuilder.RenameTable(
                name: "futsal_matches",
                newName: "FutsalMatches");

            migrationBuilder.RenameTable(
                name: "futsal_events",
                newName: "FutsalEvents");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_players_TeamId",
                table: "FutsalPlayers",
                newName: "IX_FutsalPlayers_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_matches_WinnerTeamId",
                table: "FutsalMatches",
                newName: "IX_FutsalMatches_WinnerTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_matches_HomeTeamId",
                table: "FutsalMatches",
                newName: "IX_FutsalMatches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_matches_EventId",
                table: "FutsalMatches",
                newName: "IX_FutsalMatches_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_matches_AwayTeamId",
                table: "FutsalMatches",
                newName: "IX_FutsalMatches_AwayTeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FutsalTeams",
                table: "FutsalTeams",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FutsalPlayers",
                table: "FutsalPlayers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FutsalMatches",
                table: "FutsalMatches",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FutsalEvents",
                table: "FutsalEvents",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalEvents_events_id",
                table: "FutsalEvents",
                column: "id",
                principalTable: "events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalMatches_FutsalEvents_EventId",
                table: "FutsalMatches",
                column: "EventId",
                principalTable: "FutsalEvents",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_AwayTeamId",
                table: "FutsalMatches",
                column: "AwayTeamId",
                principalTable: "FutsalTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_HomeTeamId",
                table: "FutsalMatches",
                column: "HomeTeamId",
                principalTable: "FutsalTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalMatches_FutsalTeams_WinnerTeamId",
                table: "FutsalMatches",
                column: "WinnerTeamId",
                principalTable: "FutsalTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalPlayers_FutsalTeams_TeamId",
                table: "FutsalPlayers",
                column: "TeamId",
                principalTable: "FutsalTeams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FutsalPlayers_persons_id",
                table: "FutsalPlayers",
                column: "id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
