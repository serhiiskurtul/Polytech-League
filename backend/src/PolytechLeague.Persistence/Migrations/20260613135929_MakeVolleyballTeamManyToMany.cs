using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeVolleyballTeamManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "volleyball_team_players",
                columns: table => new
                {
                    PlayersId = table.Column<long>(type: "bigint", nullable: false),
                    VolleyballTeamsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_team_players", x => new { x.PlayersId, x.VolleyballTeamsId });
                    table.ForeignKey(
                        name: "FK_volleyball_team_players_players_PlayersId",
                        column: x => x.PlayersId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_team_players_volleyball_teams_VolleyballTeamsId",
                        column: x => x.VolleyballTeamsId,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_team_players_VolleyballTeamsId",
                table: "volleyball_team_players",
                column: "VolleyballTeamsId");

            migrationBuilder.Sql(
                "INSERT INTO volleyball_team_players (\"PlayersId\", \"VolleyballTeamsId\") SELECT id, volleyball_team_id FROM players WHERE volleyball_team_id IS NOT NULL;");

            migrationBuilder.DropForeignKey(
                name: "FK_players_volleyball_teams_volleyball_team_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_volleyball_team_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "volleyball_team_id",
                table: "players");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "volleyball_team_id",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_volleyball_team_id",
                table: "players",
                column: "volleyball_team_id");

            migrationBuilder.Sql(
                "UPDATE players SET volleyball_team_id = vtp.\"VolleyballTeamsId\" FROM volleyball_team_players vtp WHERE players.id = vtp.\"PlayersId\";");

            migrationBuilder.AddForeignKey(
                name: "FK_players_volleyball_teams_volleyball_team_id",
                table: "players",
                column: "volleyball_team_id",
                principalTable: "volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.DropTable(
                name: "volleyball_team_players");
        }
    }
}
