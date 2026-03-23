using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class TABLE_TENIS_ADD_ENTITIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");

            migrationBuilder.CreateTable(
                name: "table_tennis_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    tournament_type = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    playing_hand = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_players_players_id",
                        column: x => x.id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_event_participants",
                columns: table => new
                {
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    player_id = table.Column<long>(type: "bigint", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false, defaultValue: 0.0),
                    rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_event_participants", x => new { x.tournament_id, x.player_id });
                    table.ForeignKey(
                        name: "FK_table_tennis_event_participants_table_tennis_events_tournam~",
                        column: x => x.tournament_id,
                        principalTable: "table_tennis_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_tennis_event_participants_table_tennis_players_player~",
                        column: x => x.player_id,
                        principalTable: "table_tennis_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_matches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    player1_id = table.Column<long>(type: "bigint", nullable: false),
                    player2_id = table.Column<long>(type: "bigint", nullable: false),
                    winner_id = table.Column<long>(type: "bigint", nullable: true),
                    stage = table.Column<string>(type: "text", nullable: true),
                    table_number = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    sets_to_win = table.Column<int>(type: "integer", nullable: false, defaultValue: 3)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_table_tennis_events_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "table_tennis_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_table_tennis_players_player1_id",
                        column: x => x.player1_id,
                        principalTable: "table_tennis_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_table_tennis_players_player2_id",
                        column: x => x.player2_id,
                        principalTable: "table_tennis_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_table_tennis_matches_table_tennis_players_winner_id",
                        column: x => x.winner_id,
                        principalTable: "table_tennis_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_sets",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    match_id = table.Column<long>(type: "bigint", nullable: false),
                    set_number = table.Column<int>(type: "integer", nullable: false),
                    player1_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    player2_score = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_sets", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_sets_table_tennis_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "table_tennis_matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_event_participants_player_id",
                table: "table_tennis_event_participants",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_tt_events_category",
                table: "table_tennis_events",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_tt_events_type",
                table: "table_tennis_events",
                column: "tournament_type");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_player1_id",
                table: "table_tennis_matches",
                column: "player1_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_player2_id",
                table: "table_tennis_matches",
                column: "player2_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_matches_winner_id",
                table: "table_tennis_matches",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "IX_tt_matches_tournament_id",
                table: "table_tennis_matches",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_players_rating",
                table: "table_tennis_players",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "IX_tt_sets_match_setnumber",
                table: "table_tennis_sets",
                columns: new[] { "match_id", "set_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "table_tennis_event_participants");

            migrationBuilder.DropTable(
                name: "table_tennis_sets");

            migrationBuilder.DropTable(
                name: "table_tennis_matches");

            migrationBuilder.DropTable(
                name: "table_tennis_events");

            migrationBuilder.DropTable(
                name: "table_tennis_players");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");
        }
    }
}
