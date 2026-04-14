using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UNIFY_BADMINTON_TEAM_IDS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team1Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team2Id",
                table: "badminton_matches");

            migrationBuilder.RenameColumn(
                name: "Team2Id",
                table: "badminton_matches",
                newName: "HomeTeamId");

            migrationBuilder.RenameColumn(
                name: "Team1Id",
                table: "badminton_matches",
                newName: "AwayTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_Team2Id",
                table: "badminton_matches",
                newName: "IX_badminton_matches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_Team1Id",
                table: "badminton_matches",
                newName: "IX_badminton_matches_AwayTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_AwayTeamId",
                table: "badminton_matches",
                column: "AwayTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_HomeTeamId",
                table: "badminton_matches",
                column: "HomeTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_AwayTeamId",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_HomeTeamId",
                table: "badminton_matches");

            migrationBuilder.RenameColumn(
                name: "HomeTeamId",
                table: "badminton_matches",
                newName: "Team2Id");

            migrationBuilder.RenameColumn(
                name: "AwayTeamId",
                table: "badminton_matches",
                newName: "Team1Id");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_HomeTeamId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_Team2Id");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_AwayTeamId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_Team1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team1Id",
                table: "badminton_matches",
                column: "Team1Id",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team2Id",
                table: "badminton_matches",
                column: "Team2Id",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
