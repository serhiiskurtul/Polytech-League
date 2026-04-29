using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddFutsalRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches",
                column: "AwayCaptainId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches",
                column: "AwayGoalkeeperId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches",
                column: "HomeCaptainId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches",
                column: "HomeGoalkeeperId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches",
                column: "AwayCaptainId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches",
                column: "AwayGoalkeeperId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches",
                column: "HomeCaptainId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches",
                column: "HomeGoalkeeperId",
                principalTable: "players",
                principalColumn: "id");
        }
    }
}
