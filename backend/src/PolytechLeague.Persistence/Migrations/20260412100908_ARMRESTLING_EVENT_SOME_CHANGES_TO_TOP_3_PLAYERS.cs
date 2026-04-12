using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ARMRESTLING_EVENT_SOME_CHANGES_TO_TOP_3_PLAYERS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_category", "men,women,open")
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
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
                name: "TableNumber",
                table: "armwrestling_matches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceId",
                table: "armwrestling_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceId",
                table: "armwrestling_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceId",
                table: "armwrestling_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_events_FirstPlaceId",
                table: "armwrestling_events",
                column: "FirstPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_events_SecondPlaceId",
                table: "armwrestling_events",
                column: "SecondPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_events_ThirdPlaceId",
                table: "armwrestling_events",
                column: "ThirdPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_events_players_FirstPlaceId",
                table: "armwrestling_events",
                column: "FirstPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_events_players_SecondPlaceId",
                table: "armwrestling_events",
                column: "SecondPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_events_players_ThirdPlaceId",
                table: "armwrestling_events",
                column: "ThirdPlaceId",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_events_players_FirstPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_events_players_SecondPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_events_players_ThirdPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropIndex(
                name: "IX_armwrestling_events_FirstPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropIndex(
                name: "IX_armwrestling_events_SecondPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropIndex(
                name: "IX_armwrestling_events_ThirdPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "armwrestling_matches");

            migrationBuilder.DropColumn(
                name: "FirstPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceId",
                table: "armwrestling_events");

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
                .OldAnnotation("Npgsql:Enum:armwrestling_category", "men,women,open")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
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
        }
    }
}
