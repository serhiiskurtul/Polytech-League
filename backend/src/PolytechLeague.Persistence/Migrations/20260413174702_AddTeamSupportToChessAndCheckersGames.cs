using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamSupportToChessAndCheckersGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BlackTeamId",
                table: "chess_games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BlackTeamScore",
                table: "chess_games",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WhiteTeamId",
                table: "chess_games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WhiteTeamScore",
                table: "chess_games",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BlackTeamId",
                table: "checkers_games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "BlackTeamScore",
                table: "checkers_games",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WhiteTeamId",
                table: "checkers_games",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "WhiteTeamScore",
                table: "checkers_games",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_BlackTeamId",
                table: "chess_games",
                column: "BlackTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_WhiteTeamId",
                table: "chess_games",
                column: "WhiteTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_BlackTeamId",
                table: "checkers_games",
                column: "BlackTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_WhiteTeamId",
                table: "checkers_games",
                column: "WhiteTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_BlackTeamId",
                table: "checkers_games",
                column: "BlackTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_WhiteTeamId",
                table: "checkers_games",
                column: "WhiteTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_BlackTeamId",
                table: "chess_games",
                column: "BlackTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_WhiteTeamId",
                table: "chess_games",
                column: "WhiteTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_BlackTeamId",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_WhiteTeamId",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_BlackTeamId",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_WhiteTeamId",
                table: "chess_games");

            migrationBuilder.DropIndex(
                name: "IX_chess_games_BlackTeamId",
                table: "chess_games");

            migrationBuilder.DropIndex(
                name: "IX_chess_games_WhiteTeamId",
                table: "chess_games");

            migrationBuilder.DropIndex(
                name: "IX_checkers_games_BlackTeamId",
                table: "checkers_games");

            migrationBuilder.DropIndex(
                name: "IX_checkers_games_WhiteTeamId",
                table: "checkers_games");

            migrationBuilder.DropColumn(
                name: "BlackTeamId",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "BlackTeamScore",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "WhiteTeamId",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "WhiteTeamScore",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "BlackTeamId",
                table: "checkers_games");

            migrationBuilder.DropColumn(
                name: "BlackTeamScore",
                table: "checkers_games");

            migrationBuilder.DropColumn(
                name: "WhiteTeamId",
                table: "checkers_games");

            migrationBuilder.DropColumn(
                name: "WhiteTeamScore",
                table: "checkers_games");
        }
    }
}
