using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_FUTSAL_MATCH_CAPTAINS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AwayCaptainId",
                table: "futsal_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HomeCaptainId",
                table: "futsal_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_AwayCaptainId",
                table: "futsal_matches",
                column: "AwayCaptainId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_matches_HomeCaptainId",
                table: "futsal_matches",
                column: "HomeCaptainId");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches",
                column: "AwayCaptainId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches",
                column: "HomeCaptainId",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_AwayCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_matches_players_HomeCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropIndex(
                name: "IX_futsal_matches_AwayCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropIndex(
                name: "IX_futsal_matches_HomeCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropColumn(
                name: "AwayCaptainId",
                table: "futsal_matches");

            migrationBuilder.DropColumn(
                name: "HomeCaptainId",
                table: "futsal_matches");
        }
    }
}
