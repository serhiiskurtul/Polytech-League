using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableTennisTeamsV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "volleyball_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "player2_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "player1_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "away_team_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "away_team_score",
                table: "table_tennis_matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "home_team_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "home_team_score",
                table: "table_tennis_matches",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "winner_team_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "first_place_team_id",
                table: "table_tennis_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "second_place_team_id",
                table: "table_tennis_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "third_place_team_id",
                table: "table_tennis_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "futsal_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "beach_volleyball_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "basketball3x3_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "badminton_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "armwrestling_events",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "table_tennis_teams",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    event_id = table.Column<long>(type: "bigint", nullable: false),
                    points = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    rank = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_teams_table_tennis_events_event_id",
                        column: x => x.event_id,
                        principalTable: "table_tennis_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_team_players",
                columns: table => new
                {
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    team_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_team_players", x => new { x.player_id, x.team_id });
                    table.ForeignKey(
                        name: "FK_table_tennis_team_players_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_tennis_team_players_table_tennis_teams_team_id",
                        column: x => x.team_id,
                        principalTable: "table_tennis_teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_away_team_id",
                table: "table_tennis_matches",
                column: "away_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_home_team_id",
                table: "table_tennis_matches",
                column: "home_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_winner_team_id",
                table: "table_tennis_matches",
                column: "winner_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_events_first_place_team_id",
                table: "table_tennis_events",
                column: "first_place_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_events_second_place_team_id",
                table: "table_tennis_events",
                column: "second_place_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_events_third_place_team_id",
                table: "table_tennis_events",
                column: "third_place_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_team_players_team_id",
                table: "table_tennis_team_players",
                column: "team_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_teams_event_id",
                table: "table_tennis_teams",
                column: "event_id");

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_first_place_team_id",
                table: "table_tennis_events",
                column: "first_place_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_second_place_team_id",
                table: "table_tennis_events",
                column: "second_place_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_third_place_team_id",
                table: "table_tennis_events",
                column: "third_place_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_away_team_id",
                table: "table_tennis_matches",
                column: "away_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_home_team_id",
                table: "table_tennis_matches",
                column: "home_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_winner_team_id",
                table: "table_tennis_matches",
                column: "winner_team_id",
                principalTable: "table_tennis_teams",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_first_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_second_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_events_table_tennis_teams_third_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_away_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_home_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_teams_winner_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropTable(
                name: "table_tennis_team_players");

            migrationBuilder.DropTable(
                name: "table_tennis_teams");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_matches_away_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_matches_home_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_matches_winner_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_events_first_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_events_second_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_events_third_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropColumn(
                name: "away_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropColumn(
                name: "away_team_score",
                table: "table_tennis_matches");

            migrationBuilder.DropColumn(
                name: "home_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropColumn(
                name: "home_team_score",
                table: "table_tennis_matches");

            migrationBuilder.DropColumn(
                name: "winner_team_id",
                table: "table_tennis_matches");

            migrationBuilder.DropColumn(
                name: "first_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropColumn(
                name: "second_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.DropColumn(
                name: "third_place_team_id",
                table: "table_tennis_events");

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "volleyball_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "player2_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "player1_id",
                table: "table_tennis_matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "futsal_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "beach_volleyball_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "basketball3x3_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "badminton_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TournamentType",
                table: "armwrestling_events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }
    }
}
