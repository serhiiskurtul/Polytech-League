using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyFootballTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_Football_teams_Football_team_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_Football_team_id",
                table: "players");

            migrationBuilder.CreateTable(
                name: "football_team_players",
                columns: table => new
                {
                    FootballTeamsId = table.Column<long>(type: "bigint", nullable: false),
                    PlayersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_football_team_players", x => new { x.FootballTeamsId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_football_team_players_Football_teams_FootballTeamsId",
                        column: x => x.FootballTeamsId,
                        principalTable: "Football_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_football_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_football_team_players_PlayersId",
                table: "football_team_players",
                column: "PlayersId");

            migrationBuilder.Sql(
                "INSERT INTO football_team_players (\"FootballTeamsId\", \"PlayersId\") SELECT \"Football_team_id\", \"id\" FROM players WHERE \"Football_team_id\" IS NOT NULL;");

            migrationBuilder.DropColumn(
                name: "Football_team_id",
                table: "players");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "football_team_players");

            migrationBuilder.AddColumn<long>(
                name: "Football_team_id",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_Football_team_id",
                table: "players",
                column: "Football_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_Football_teams_Football_team_id",
                table: "players",
                column: "Football_team_id",
                principalTable: "Football_teams",
                principalColumn: "Id");
        }
    }
}
