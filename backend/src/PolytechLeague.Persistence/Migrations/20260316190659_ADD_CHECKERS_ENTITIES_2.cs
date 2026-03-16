using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_CHECKERS_ENTITIES_2 : Migration
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
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");

            migrationBuilder.CreateTable(
                name: "checkers_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    time_control = table.Column<string>(type: "text", nullable: false),
                    participants_count = table.Column<int>(type: "integer", nullable: false),
                    system = table.Column<int>(type: "integer", nullable: false),
                    variant = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_referees",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_referees", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_referees_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_event_participants",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CheckersEventId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    starting_rating = table.Column<int>(type: "integer", nullable: false),
                    points_earned = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_event_participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checkers_event_participants_checkers_events_CheckersEventId",
                        column: x => x.CheckersEventId,
                        principalTable: "checkers_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_checkers_event_participants_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_games",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    checkers_event_id = table.Column<long>(type: "bigint", nullable: false),
                    round_number = table.Column<int>(type: "integer", nullable: false),
                    white_player_id = table.Column<long>(type: "bigint", nullable: false),
                    black_player_id = table.Column<long>(type: "bigint", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false),
                    scheduled_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_event_participants_black_player_id",
                        column: x => x.black_player_id,
                        principalTable: "checkers_event_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_event_participants_white_player_id",
                        column: x => x.white_player_id,
                        principalTable: "checkers_event_participants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_checkers_games_checkers_events_checkers_event_id",
                        column: x => x.checkers_event_id,
                        principalTable: "checkers_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_id",
                table: "checkers_event_participants",
                column: "CheckersEventId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_points",
                table: "checkers_event_participants",
                columns: new[] { "CheckersEventId", "points_earned" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_event_user",
                table: "checkers_event_participants",
                columns: new[] { "CheckersEventId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_user_id",
                table: "checkers_event_participants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_system",
                table: "checkers_events",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_variant",
                table: "checkers_events",
                column: "variant");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_black_player_id",
                table: "checkers_games",
                column: "black_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_black",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "black_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_id",
                table: "checkers_games",
                column: "checkers_event_id");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_round",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "round_number" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_event_white",
                table: "checkers_games",
                columns: new[] { "checkers_event_id", "white_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_result",
                table: "checkers_games",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_games_white_player_id",
                table: "checkers_games",
                column: "white_player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkers_games");

            migrationBuilder.DropTable(
                name: "checkers_referees");

            migrationBuilder.DropTable(
                name: "checkers_event_participants");

            migrationBuilder.DropTable(
                name: "checkers_events");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");
        }
    }
}
