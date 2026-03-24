using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class BASKETBALL_ENTITIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
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
                .OldAnnotation("Npgsql:Enum:armwrestling_category", "men,women")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");

            migrationBuilder.CreateTable(
                name: "basketball3x3_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TournamentType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_basketball3x3_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_teams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_matches",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamId = table.Column<long>(type: "bigint", nullable: false),
                    AwayTeamId = table.Column<long>(type: "bigint", nullable: false),
                    HomeTeamScore = table.Column<int>(type: "integer", nullable: true),
                    AwayTeamScore = table.Column<int>(type: "integer", nullable: true),
                    WinnerTeamId = table.Column<long>(type: "bigint", nullable: true),
                    Stage = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_events_EventId",
                        column: x => x.EventId,
                        principalTable: "basketball3x3_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_basketball3x3_matches_basketball3x3_teams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "basketball3x3_teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_AwayTeamId",
                table: "basketball3x3_matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_EventId",
                table: "basketball3x3_matches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_HomeTeamId",
                table: "basketball3x3_matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_matches_WinnerTeamId",
                table: "basketball3x3_matches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_players_TeamId",
                table: "basketball3x3_players",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basketball3x3_matches");

            migrationBuilder.DropTable(
                name: "basketball3x3_players");

            migrationBuilder.DropTable(
                name: "basketball3x3_events");

            migrationBuilder.DropTable(
                name: "basketball3x3_teams");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
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
                .OldAnnotation("Npgsql:Enum:armwrestling_category", "men,women")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");
        }
    }
}
