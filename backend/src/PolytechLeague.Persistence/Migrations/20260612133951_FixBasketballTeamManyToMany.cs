using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixBasketballTeamManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_basketball3x3_teams_basketball_team_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_basketball_team_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "basketball_team_id",
                table: "players");

            migrationBuilder.CreateTable(
                name: "basketball_team_players",
                columns: table => new
                {
                    BasketballTeamsId = table.Column<long>(type: "bigint", nullable: false),
                    PlayersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball_team_players", x => new { x.BasketballTeamsId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_basketball_team_players_basketball3x3_teams_BasketballTeams~",
                        column: x => x.BasketballTeamsId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_basketball_team_players_PlayersId",
                table: "basketball_team_players",
                column: "PlayersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basketball_team_players");

            migrationBuilder.AddColumn<long>(
                name: "basketball_team_id",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_basketball_team_id",
                table: "players",
                column: "basketball_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_basketball3x3_teams_basketball_team_id",
                table: "players",
                column: "basketball_team_id",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");
        }
    }
}
