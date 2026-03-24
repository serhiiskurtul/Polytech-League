using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FUTSAL_ADD_ENTIIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");

            migrationBuilder.CreateTable(
                name: "FutsalEvents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutsalEvents", x => x.id);
                    table.ForeignKey(
                        name: "FK_FutsalEvents_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FutsalTeams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutsalTeams", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FutsalMatches",
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
                    table.PrimaryKey("PK_FutsalMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FutsalMatches_FutsalEvents_EventId",
                        column: x => x.EventId,
                        principalTable: "FutsalEvents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FutsalMatches_FutsalTeams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "FutsalTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FutsalMatches_FutsalTeams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "FutsalTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FutsalMatches_FutsalTeams_WinnerTeamId",
                        column: x => x.WinnerTeamId,
                        principalTable: "FutsalTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "FutsalPlayers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    TeamId = table.Column<long>(type: "bigint", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    ShirtNumber = table.Column<int>(type: "integer", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FutsalPlayers", x => x.id);
                    table.ForeignKey(
                        name: "FK_FutsalPlayers_FutsalTeams_TeamId",
                        column: x => x.TeamId,
                        principalTable: "FutsalTeams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_FutsalPlayers_persons_id",
                        column: x => x.id,
                        principalTable: "persons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FutsalMatches_AwayTeamId",
                table: "FutsalMatches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FutsalMatches_EventId",
                table: "FutsalMatches",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_FutsalMatches_HomeTeamId",
                table: "FutsalMatches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FutsalMatches_WinnerTeamId",
                table: "FutsalMatches",
                column: "WinnerTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_FutsalPlayers_TeamId",
                table: "FutsalPlayers",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FutsalMatches");

            migrationBuilder.DropTable(
                name: "FutsalPlayers");

            migrationBuilder.DropTable(
                name: "FutsalEvents");

            migrationBuilder.DropTable(
                name: "FutsalTeams");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
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
        }
    }
}
