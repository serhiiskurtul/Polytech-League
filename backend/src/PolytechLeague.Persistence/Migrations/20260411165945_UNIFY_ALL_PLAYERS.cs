using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UNIFY_ALL_PLAYERS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_player1_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_player2_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_winner_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_event_participants_badminton_players_PlayerId",
                table: "badminton_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_players_Player1Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_players_Player2Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_players_WinnerId",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_checkers_players_PlayerId",
                table: "checkers_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_event_participants_chess_players_PlayerId",
                table: "chess_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_players_black_player_id",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_players_white_player_id",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_event_participants_table_tennis_players_player~",
                table: "table_tennis_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_player1_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_player2_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_winner_id",
                table: "table_tennis_matches");

            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    city = table.Column<string>(type: "text", nullable: false),
                    @group = table.Column<string>(type: "text", nullable: false),
                    chess_rating = table.Column<int>(type: "integer", nullable: true),
                    chess_fide_id = table.Column<string>(type: "text", nullable: true),
                    chess_title = table.Column<string>(type: "text", nullable: true),
                    checkers_rating = table.Column<int>(type: "integer", nullable: true),
                    table_tennis_rating = table.Column<int>(type: "integer", nullable: true),
                    table_tennis_hand = table.Column<int>(type: "integer", nullable: true),
                    armwrestling_weight = table.Column<float>(type: "real", nullable: true),
                    armwrestling_preferred_weight_class = table.Column<int>(type: "integer", nullable: true),
                    futsal_position = table.Column<int>(type: "integer", nullable: true),
                    futsal_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    futsal_team_id = table.Column<long>(type: "bigint", nullable: true),
                    volleyball_position = table.Column<int>(type: "integer", nullable: true),
                    volleyball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    volleyball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    basketball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    basketball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    beach_volleyball_shirt_number = table.Column<int>(type: "integer", nullable: true),
                    beach_volleyball_team_id = table.Column<long>(type: "bigint", nullable: true),
                    Basketball3x3TeamId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_players_basketball3x3_teams_Basketball3x3TeamId",
                        column: x => x.Basketball3x3TeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_beach_volleyball_teams_beach_volleyball_team_id",
                        column: x => x.beach_volleyball_team_id,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_futsal_teams_futsal_team_id",
                        column: x => x.futsal_team_id,
                        principalTable: "futsal_teams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_players_volleyball_teams_volleyball_team_id",
                        column: x => x.volleyball_team_id,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id");
                });

            // Data Migration SQL
            // Chess
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", chess_rating, chess_fide_id, chess_title)
                SELECT id, ""City"", ""Group"", rating_elo, fide_id, title FROM chess_players
                ON CONFLICT (id) DO UPDATE SET 
                    chess_rating = EXCLUDED.chess_rating, 
                    chess_fide_id = EXCLUDED.chess_fide_id, 
                    chess_title = EXCLUDED.chess_title;
            ");

            // Checkers
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", checkers_rating)
                SELECT id, ""City"", ""Group"", rating FROM checkers_players
                ON CONFLICT (id) DO UPDATE SET 
                    checkers_rating = EXCLUDED.checkers_rating;
            ");

            // Table Tennis
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", table_tennis_rating, table_tennis_hand)
                SELECT id, ""City"", ""Group"", rating, playing_hand FROM table_tennis_players
                ON CONFLICT (id) DO UPDATE SET 
                    table_tennis_rating = EXCLUDED.table_tennis_rating, 
                    table_tennis_hand = EXCLUDED.table_tennis_hand;
            ");

            // Armwrestling
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", armwrestling_weight, armwrestling_preferred_weight_class)
                SELECT id, ""City"", ""Group"", current_weight, preferred_weight_class FROM armwrestling_players
                ON CONFLICT (id) DO UPDATE SET 
                    armwrestling_weight = EXCLUDED.armwrestling_weight, 
                    armwrestling_preferred_weight_class = EXCLUDED.armwrestling_preferred_weight_class;
            ");

            // Futsal
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", futsal_position, futsal_shirt_number, futsal_team_id)
                SELECT id, ""City"", ""Group"", ""Position"", ""ShirtNumber"", ""TeamId"" FROM futsal_players
                ON CONFLICT (id) DO UPDATE SET 
                    futsal_position = EXCLUDED.futsal_position, 
                    futsal_shirt_number = EXCLUDED.futsal_shirt_number, 
                    futsal_team_id = EXCLUDED.futsal_team_id;
            ");

            // Volleyball
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", volleyball_position, volleyball_shirt_number, volleyball_team_id)
                SELECT id, ""City"", ""Group"", ""Position"", ""ShirtNumber"", ""TeamId"" FROM volleyball_players
                ON CONFLICT (id) DO UPDATE SET 
                    volleyball_position = EXCLUDED.volleyball_position, 
                    volleyball_shirt_number = EXCLUDED.volleyball_shirt_number, 
                    volleyball_team_id = EXCLUDED.volleyball_team_id;
            ");

            // Basketball
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", basketball_shirt_number, basketball_team_id)
                SELECT id, ""City"", ""Group"", ""ShirtNumber"", ""TeamId"" FROM basketball3x3_players
                ON CONFLICT (id) DO UPDATE SET 
                    basketball_shirt_number = EXCLUDED.basketball_shirt_number, 
                    basketball_team_id = EXCLUDED.basketball_team_id;
            ");

            // Beach Volleyball
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"", beach_volleyball_shirt_number, beach_volleyball_team_id)
                SELECT id, ""City"", ""Group"", ""ShirtNumber"", ""TeamId"" FROM beach_volleyball_players
                ON CONFLICT (id) DO UPDATE SET 
                    beach_volleyball_shirt_number = EXCLUDED.beach_volleyball_shirt_number, 
                    beach_volleyball_team_id = EXCLUDED.beach_volleyball_team_id;
            ");

            // Badminton
            migrationBuilder.Sql(@"
                INSERT INTO players (id, city, ""group"")
                SELECT id, ""City"", ""Group"" FROM badminton_players
                ON CONFLICT (id) DO NOTHING;
            ");

            migrationBuilder.DropTable(name: "armwrestling_players");
            migrationBuilder.DropTable(name: "badminton_players");
            migrationBuilder.DropTable(name: "basketball3x3_players");
            migrationBuilder.DropTable(name: "beach_volleyball_players");
            migrationBuilder.DropTable(name: "checkers_players");
            migrationBuilder.DropTable(name: "chess_players");
            migrationBuilder.DropTable(name: "futsal_players");
            migrationBuilder.DropTable(name: "table_tennis_players");
            migrationBuilder.DropTable(name: "volleyball_players");

            migrationBuilder.CreateIndex(
                name: "IX_players_Basketball3x3TeamId",
                table: "players",
                column: "Basketball3x3TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_beach_volleyball_team_id",
                table: "players",
                column: "beach_volleyball_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_futsal_team_id",
                table: "players",
                column: "futsal_team_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_volleyball_team_id",
                table: "players",
                column: "volleyball_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_player1_id",
                table: "armwrestling_matches",
                column: "player1_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_player2_id",
                table: "armwrestling_matches",
                column: "player2_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_players_winner_id",
                table: "armwrestling_matches",
                column: "winner_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_players_PlayerId",
                table: "badminton_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_Player1Id",
                table: "badminton_matches",
                column: "Player1Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_Player2Id",
                table: "badminton_matches",
                column: "Player2Id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_WinnerId",
                table: "badminton_matches",
                column: "WinnerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_players_PlayerId",
                table: "checkers_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_event_participants_players_PlayerId",
                table: "chess_event_participants",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_players_black_player_id",
                table: "chess_games",
                column: "black_player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_players_white_player_id",
                table: "chess_games",
                column: "white_player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_event_participants_players_player_id",
                table: "table_tennis_event_participants",
                column: "player_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_players_player1_id",
                table: "table_tennis_matches",
                column: "player1_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_players_player2_id",
                table: "table_tennis_matches",
                column: "player2_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_players_winner_id",
                table: "table_tennis_matches",
                column: "winner_id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_players_player1_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_players_player2_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_matches_players_winner_id",
                table: "armwrestling_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_event_participants_players_PlayerId",
                table: "badminton_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_players_Player1Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_players_Player2Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_players_WinnerId",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_players_PlayerId",
                table: "checkers_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_event_participants_players_PlayerId",
                table: "chess_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_players_black_player_id",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_players_white_player_id",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_event_participants_players_player_id",
                table: "table_tennis_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_players_player1_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_players_player2_id",
                table: "table_tennis_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_matches_players_winner_id",
                table: "table_tennis_matches");

            migrationBuilder.DropTable(
                name: "players");

            migrationBuilder.CreateTable(
                name: "armwrestling_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    current_weight = table.Column<float>(type: "real", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    preferred_weight_class = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_armwrestling_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_badminton_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_basketball3x3_players_basketball3x3_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_basketball3x3_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_players_beach_volleyball_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "beach_volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "checkers_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_checkers_players_persons_id",
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
                    City = table.Column<string>(type: "text", nullable: false),
                    fide_id = table.Column<string>(type: "text", nullable: true),
                    Group = table.Column<string>(type: "text", nullable: false),
                    rating_elo = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_chess_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_futsal_players_futsal_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "futsal_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_futsal_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_tennis_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    playing_hand = table.Column<int>(type: "integer", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_table_tennis_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_table_tennis_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_players", x => x.id);
                    table.ForeignKey(
                        name: "FK_volleyball_players_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_players_volleyball_teams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "volleyball_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_arm_players_weight_class",
                table: "armwrestling_players",
                column: "preferred_weight_class");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_players_TeamId",
                table: "basketball3x3_players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_players_TeamId",
                table: "beach_volleyball_players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_players_rating",
                table: "checkers_players",
                column: "rating");

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
                name: "IX_futsal_players_TeamId",
                table: "futsal_players",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_players_rating",
                table: "table_tennis_players",
                column: "rating");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_players_TeamId",
                table: "volleyball_players",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_player1_id",
                table: "armwrestling_matches",
                column: "player1_id",
                principalTable: "armwrestling_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_player2_id",
                table: "armwrestling_matches",
                column: "player2_id",
                principalTable: "armwrestling_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_matches_armwrestling_players_winner_id",
                table: "armwrestling_matches",
                column: "winner_id",
                principalTable: "armwrestling_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_badminton_players_PlayerId",
                table: "badminton_event_participants",
                column: "PlayerId",
                principalTable: "badminton_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_players_Player1Id",
                table: "badminton_matches",
                column: "Player1Id",
                principalTable: "badminton_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_players_Player2Id",
                table: "badminton_matches",
                column: "Player2Id",
                principalTable: "badminton_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_players_WinnerId",
                table: "badminton_matches",
                column: "WinnerId",
                principalTable: "badminton_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_checkers_players_PlayerId",
                table: "checkers_event_participants",
                column: "PlayerId",
                principalTable: "checkers_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_event_participants_chess_players_PlayerId",
                table: "chess_event_participants",
                column: "PlayerId",
                principalTable: "chess_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_players_black_player_id",
                table: "chess_games",
                column: "black_player_id",
                principalTable: "chess_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_players_white_player_id",
                table: "chess_games",
                column: "white_player_id",
                principalTable: "chess_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_event_participants_table_tennis_players_player~",
                table: "table_tennis_event_participants",
                column: "player_id",
                principalTable: "table_tennis_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_player1_id",
                table: "table_tennis_matches",
                column: "player1_id",
                principalTable: "table_tennis_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_player2_id",
                table: "table_tennis_matches",
                column: "player2_id",
                principalTable: "table_tennis_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_matches_table_tennis_players_winner_id",
                table: "table_tennis_matches",
                column: "winner_id",
                principalTable: "table_tennis_players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
