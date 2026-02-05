using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampusLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitChessGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:chess_game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:user_role", "user,admin,none");

            migrationBuilder.CreateTable(
                name: "events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    location = table.Column<string>(type: "text", nullable: false),
                    referee_ids = table.Column<long[]>(type: "bigint[]", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    birthday = table.Column<DateOnly>(type: "date", nullable: false),
                    photo_url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_persons", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    token = table.Column<string>(type: "text", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_revoked = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_tokens", x => x.id);
                    table.CheckConstraint("CK_RefreshToken_ExpiresAt", "expires_at >= now()");
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.CheckConstraint("CK_User_CreatedAt", "created_at <= now()");
                });

            migrationBuilder.CreateTable(
                name: "chess_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    time_control = table.Column<string>(type: "text", nullable: false),
                    participants_count = table.Column<int>(type: "integer", nullable: false),
                    system = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "event_photos",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    url = table.Column<string>(type: "text", nullable: false),
                    event_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_photos", x => x.id);
                    table.ForeignKey(
                        name: "FK_event_photos_events_event_id",
                        column: x => x.event_id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_referees",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_referees", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_referees_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    group = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    rating_elo = table.Column<int>(type: "integer", nullable: false),
                    fide_id = table.Column<string>(type: "text", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_players_players_id",
                        column: x => x.id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_event_participants",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    points = table.Column<double>(type: "double precision", nullable: false),
                    rank = table.Column<int>(type: "integer", nullable: false),
                    ChessEventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_event_participants", x => new { x.TournamentId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_chess_event_participants_chess_events_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chess_event_participants_chess_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "chess_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_games",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tournament_id = table.Column<long>(type: "bigint", nullable: false),
                    white_player_id = table.Column<long>(type: "bigint", nullable: false),
                    black_player_id = table.Column<long>(type: "bigint", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false),
                    round = table.Column<int>(type: "integer", nullable: false),
                    board = table.Column<int>(type: "integer", nullable: false),
                    ChessEventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_events_ChessEventId",
                        column: x => x.ChessEventId,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_events_tournament_id",
                        column: x => x.tournament_id,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_players_black_player_id",
                        column: x => x.black_player_id,
                        principalTable: "chess_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_chess_games_chess_players_white_player_id",
                        column: x => x.white_player_id,
                        principalTable: "chess_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_player_id",
                table: "chess_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_tournament_id",
                table: "chess_event_participants",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_tournament_points_rank",
                table: "chess_event_participants",
                columns: new[] { "TournamentId", "points", "rank" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_system",
                table: "chess_events",
                column: "system");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_black_player_id",
                table: "chess_games",
                column: "black_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_ChessEventId",
                table: "chess_games",
                column: "ChessEventId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_result",
                table: "chess_games",
                column: "result");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_black",
                table: "chess_games",
                columns: new[] { "tournament_id", "black_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_id",
                table: "chess_games",
                column: "tournament_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_round",
                table: "chess_games",
                columns: new[] { "tournament_id", "round" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_tournament_white",
                table: "chess_games",
                columns: new[] { "tournament_id", "white_player_id" });

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_white_player_id",
                table: "chess_games",
                column: "white_player_id");

            migrationBuilder.CreateIndex(
                name: "IX_chess_players_fide_id",
                table: "chess_players",
                column: "fide_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_chess_players_rating_elo",
                table: "chess_players",
                column: "rating_elo");

            migrationBuilder.CreateIndex(
                name: "IX_event_photos_event_id",
                table: "event_photos",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_events_date",
                table: "events",
                column: "date");

            migrationBuilder.CreateIndex(
                name: "IX_events_level",
                table: "events",
                column: "level");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_user_id_active",
                table: "refresh_tokens",
                columns: new[] { "user_id", "is_revoked", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "is_deleted = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chess_event_participants");

            migrationBuilder.DropTable(
                name: "chess_games");

            migrationBuilder.DropTable(
                name: "chess_referees");

            migrationBuilder.DropTable(
                name: "event_photos");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "chess_events");

            migrationBuilder.DropTable(
                name: "chess_players");

            migrationBuilder.DropTable(
                name: "events");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.DropTable(
                name: "persons");
        }
    }
}
