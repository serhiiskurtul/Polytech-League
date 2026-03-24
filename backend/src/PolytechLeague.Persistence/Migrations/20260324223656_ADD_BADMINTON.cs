using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_BADMINTON : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Position",
                table: "beach_volleyball_players");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .Annotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .Annotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .Annotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:armwrestling_category", "men,women")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:beach_volleyball_position", "blocker,defender,universal")
                .OldAnnotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.CreateTable(
                name: "badminton_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_badminton_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
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
                name: "badminton_event_participants",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Points = table.Column<double>(type: "double precision", nullable: false),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_event_participants", x => new { x.TournamentId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_badminton_event_participants_badminton_events_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "badminton_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_badminton_event_participants_badminton_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "badminton_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    Player1Id = table.Column<long>(type: "bigint", nullable: false),
                    Player2Id = table.Column<long>(type: "bigint", nullable: false),
                    WinnerId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "text", nullable: true),
                    CourtNumber = table.Column<int>(type: "integer", nullable: false),
                    SetsToWin = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_matches_badminton_events_TournamentId",
                        column: x => x.TournamentId,
                        principalTable: "badminton_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_badminton_matches_badminton_players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "badminton_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_badminton_matches_badminton_players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "badminton_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_badminton_matches_badminton_players_WinnerId",
                        column: x => x.WinnerId,
                        principalTable: "badminton_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "badminton_sets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MatchId = table.Column<long>(type: "bigint", nullable: false),
                    SetNumber = table.Column<int>(type: "integer", nullable: false),
                    Player1Score = table.Column<int>(type: "integer", nullable: false),
                    Player2Score = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_sets_badminton_matches_MatchId",
                        column: x => x.MatchId,
                        principalTable: "badminton_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_badminton_event_participants_PlayerId",
                table: "badminton_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Player1Id",
                table: "badminton_matches",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Player2Id",
                table: "badminton_matches",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_TournamentId",
                table: "badminton_matches",
                column: "TournamentId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_WinnerId",
                table: "badminton_matches",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_sets_MatchId",
                table: "badminton_sets",
                column: "MatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "badminton_event_participants");

            migrationBuilder.DropTable(
                name: "badminton_sets");

            migrationBuilder.DropTable(
                name: "badminton_matches");

            migrationBuilder.DropTable(
                name: "badminton_events");

            migrationBuilder.DropTable(
                name: "badminton_players");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:beach_volleyball_position", "blocker,defender,universal")
                .Annotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .Annotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "user,admin")
                .Annotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .Annotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:armwrestling_category", "men,women")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .OldAnnotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "beach_volleyball_players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
