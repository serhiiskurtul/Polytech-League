using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeBeachVolleyballTeamManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.CreateTable(
                name: "beach_volleyball_team_players",
                columns: table => new
                {
                    BeachVolleyballTeamsId = table.Column<long>(type: "bigint", nullable: false),
                    PlayersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_team_players", x => new { x.BeachVolleyballTeamsId, x.PlayersId });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_team_players_beach_volleyball_teams_BeachV~",
                        column: x => x.BeachVolleyballTeamsId,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_team_players_PlayersId",
                table: "beach_volleyball_team_players",
                column: "PlayersId");

            migrationBuilder.Sql("INSERT INTO beach_volleyball_team_players (\"BeachVolleyballTeamsId\", \"PlayersId\") SELECT beach_volleyball_team_id, id FROM players WHERE beach_volleyball_team_id IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_players_beach_volleyball_teams_beach_volleyball_team_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_beach_volleyball_team_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "beach_volleyball_team_id",
                table: "players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "beach_volleyball_team_players");

            migrationBuilder.AddColumn<long>(
                name: "beach_volleyball_team_id",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_beach_volleyball_team_id",
                table: "players",
                column: "beach_volleyball_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_beach_volleyball_teams_beach_volleyball_team_id",
                table: "players",
                column: "beach_volleyball_team_id",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id");
        }
    }
}
