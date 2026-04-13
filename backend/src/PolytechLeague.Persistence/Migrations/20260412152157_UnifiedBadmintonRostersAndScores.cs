using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedBadmintonRostersAndScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BadmintonMatchId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BadmintonMatchId1",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AwayScore",
                table: "badminton_matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeScore",
                table: "badminton_matches",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_BadmintonMatchId",
                table: "players",
                column: "BadmintonMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_players_BadmintonMatchId1",
                table: "players",
                column: "BadmintonMatchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId",
                table: "players",
                column: "BadmintonMatchId",
                principalTable: "badminton_matches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId1",
                table: "players",
                column: "BadmintonMatchId1",
                principalTable: "badminton_matches",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId1",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_BadmintonMatchId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_BadmintonMatchId1",
                table: "players");

            migrationBuilder.DropColumn(
                name: "BadmintonMatchId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "BadmintonMatchId1",
                table: "players");

            migrationBuilder.DropColumn(
                name: "AwayScore",
                table: "badminton_matches");

            migrationBuilder.DropColumn(
                name: "HomeScore",
                table: "badminton_matches");
        }
    }
}
