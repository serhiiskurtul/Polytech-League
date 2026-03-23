using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ARMWRESTLING_ADD_ENTITIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_players_players_id",
                table: "checkers_players");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_players_players_id",
                table: "chess_players");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_players_players_id",
                table: "table_tennis_players");

            migrationBuilder.DropTable(
                name: "players");

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
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "user,admin");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "table_tennis_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "table_tennis_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "chess_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "chess_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "checkers_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "checkers_players",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "armwrestling_events",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    hand = table.Column<int>(type: "integer", nullable: false),
                    weight_class = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_events", x => x.id);
                    table.ForeignKey(
                        name: "FK_armwrestling_events_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "armwrestling_players",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    current_weight = table.Column<float>(type: "real", nullable: false),
                    preferred_weight_class = table.Column<int>(type: "integer", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    Group = table.Column<string>(type: "text", nullable: false)
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
                name: "armwrestling_matches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    event_id = table.Column<long>(type: "bigint", nullable: false),
                    player1_id = table.Column<long>(type: "bigint", nullable: false),
                    player2_id = table.Column<long>(type: "bigint", nullable: false),
                    winner_id = table.Column<long>(type: "bigint", nullable: true),
                    stage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_matches", x => x.id);
                    table.ForeignKey(
                        name: "FK_armwrestling_matches_armwrestling_events_event_id",
                        column: x => x.event_id,
                        principalTable: "armwrestling_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_armwrestling_matches_armwrestling_players_player1_id",
                        column: x => x.player1_id,
                        principalTable: "armwrestling_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_armwrestling_matches_armwrestling_players_player2_id",
                        column: x => x.player2_id,
                        principalTable: "armwrestling_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_armwrestling_matches_armwrestling_players_winner_id",
                        column: x => x.winner_id,
                        principalTable: "armwrestling_players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_category",
                table: "armwrestling_events",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_hand",
                table: "armwrestling_events",
                column: "hand");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_weight_class",
                table: "armwrestling_events",
                column: "weight_class");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_event_id",
                table: "armwrestling_matches",
                column: "event_id");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_player1_id",
                table: "armwrestling_matches",
                column: "player1_id");

            migrationBuilder.CreateIndex(
                name: "IX_arm_matches_player2_id",
                table: "armwrestling_matches",
                column: "player2_id");

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_matches_winner_id",
                table: "armwrestling_matches",
                column: "winner_id");

            migrationBuilder.CreateIndex(
                name: "IX_arm_players_weight_class",
                table: "armwrestling_players",
                column: "preferred_weight_class");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_players_persons_id",
                table: "checkers_players",
                column: "id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_players_persons_id",
                table: "chess_players",
                column: "id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_players_persons_id",
                table: "table_tennis_players",
                column: "id",
                principalTable: "persons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_players_persons_id",
                table: "checkers_players");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_players_persons_id",
                table: "chess_players");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_players_persons_id",
                table: "table_tennis_players");

            migrationBuilder.DropTable(
                name: "armwrestling_matches");

            migrationBuilder.DropTable(
                name: "armwrestling_events");

            migrationBuilder.DropTable(
                name: "armwrestling_players");

            migrationBuilder.DropColumn(
                name: "City",
                table: "table_tennis_players");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "table_tennis_players");

            migrationBuilder.DropColumn(
                name: "City",
                table: "chess_players");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "chess_players");

            migrationBuilder.DropColumn(
                name: "City",
                table: "checkers_players");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "checkers_players");

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

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_players_players_id",
                table: "checkers_players",
                column: "id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_players_players_id",
                table: "chess_players",
                column: "id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_players_players_id",
                table: "table_tennis_players",
                column: "id",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
