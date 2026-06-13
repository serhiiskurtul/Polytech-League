using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeChessAndCheckersTeamsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "checkers_team_players",
                columns: table => new
                {
                    CheckersTeamsId = table.Column<long>(type: "bigint", nullable: false),
                    PlayersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_team_players", x => new { x.CheckersTeamsId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_checkers_team_players_checkers_teams_CheckersTeamsId",
                        column: x => x.CheckersTeamsId,
                        principalTable: "checkers_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkers_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_team_players",
                columns: table => new
                {
                    ChessTeamsId = table.Column<long>(type: "bigint", nullable: false),
                    PlayersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_team_players", x => new { x.ChessTeamsId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_chess_team_players_chess_teams_ChessTeamsId",
                        column: x => x.ChessTeamsId,
                        principalTable: "chess_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chess_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_team_players_PlayersId",
                table: "checkers_team_players",
                column: "PlayersId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_team_players_PlayersId",
                table: "chess_team_players",
                column: "PlayersId");

            migrationBuilder.Sql(
                "INSERT INTO chess_team_players (\"PlayersId\", \"ChessTeamsId\") SELECT id, \"ChessTeamId\" FROM players WHERE \"ChessTeamId\" IS NOT NULL;");

            migrationBuilder.Sql(
                "INSERT INTO checkers_team_players (\"PlayersId\", \"CheckersTeamsId\") SELECT id, \"CheckersTeamId\" FROM players WHERE \"CheckersTeamId\" IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_players_checkers_teams_CheckersTeamId",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_players_chess_teams_ChessTeamId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_CheckersTeamId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_ChessTeamId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "CheckersTeamId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "ChessTeamId",
                table: "players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CheckersTeamId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChessTeamId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_CheckersTeamId",
                table: "players",
                column: "CheckersTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_ChessTeamId",
                table: "players",
                column: "ChessTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_players_checkers_teams_CheckersTeamId",
                table: "players",
                column: "CheckersTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_chess_teams_ChessTeamId",
                table: "players",
                column: "ChessTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.Sql(
                "UPDATE players SET \"ChessTeamId\" = ctp.\"ChessTeamsId\" FROM chess_team_players ctp WHERE players.id = ctp.\"PlayersId\";");

            migrationBuilder.Sql(
                "UPDATE players SET \"CheckersTeamId\" = ctp.\"CheckersTeamsId\" FROM checkers_team_players ctp WHERE players.id = ctp.\"PlayersId\";");

            migrationBuilder.DropTable(
                name: "checkers_team_players");

            migrationBuilder.DropTable(
                name: "chess_team_players");
        }
    }
}
