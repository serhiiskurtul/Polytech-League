using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCategoryToEventUnifyFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_tt_events_category",
                table: "table_tennis_events");

            migrationBuilder.DropIndex(
                name: "IX_arm_events_category",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "category",
                table: "table_tennis_events");

            migrationBuilder.DropColumn(
                name: "category",
                table: "armwrestling_events");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .Annotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:event_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .Annotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team")
                .Annotation("Npgsql:Enum:registration_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "none,user,admin")
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
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .OldAnnotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team")
                .OldAnnotation("Npgsql:Enum:registration_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "none,user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_events_category",
                table: "events",
                column: "category");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_events_category",
                table: "events");

            migrationBuilder.DropColumn(
                name: "category",
                table: "events");

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
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .Annotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team")
                .Annotation("Npgsql:Enum:registration_status", "pending,approved,rejected")
                .Annotation("Npgsql:Enum:table_tennis_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "none,user,admin")
                .Annotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .Annotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .OldAnnotation("Npgsql:Enum:basketball3x3tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:event_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:futsal_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .OldAnnotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team")
                .OldAnnotation("Npgsql:Enum:registration_status", "pending,approved,rejected")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "none,user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "table_tennis_events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "category",
                table: "armwrestling_events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tt_events_category",
                table: "table_tennis_events",
                column: "category");

            migrationBuilder.CreateIndex(
                name: "IX_arm_events_category",
                table: "armwrestling_events",
                column: "category");
        }
    }
}
