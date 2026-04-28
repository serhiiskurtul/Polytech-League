using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_GOALKEEPERS_TO_FUTSAL_MATCH : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AwayGoalkeeperId",
                table: "futsal_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HomeGoalkeeperId",
                table: "futsal_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_AwayGoalkeeperId",
                table: "futsal_matches",
                column: "AwayGoalkeeperId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_HomeGoalkeeperId",
                table: "futsal_matches",
                column: "HomeGoalkeeperId");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches",
                column: "AwayGoalkeeperId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches",
                column: "HomeGoalkeeperId",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropIndex(
                name: "IX_futsal_matches_AwayGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropIndex(
                name: "IX_futsal_matches_HomeGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropColumn(
                name: "AwayGoalkeeperId",
                table: "futsal_matches");

            migrationBuilder.DropColumn(
                name: "HomeGoalkeeperId",
                table: "futsal_matches");
        }
    }
}
