using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameBasketball3x3toBasketball : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_basketball3x3_teams_Basketball3x3TeamId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_Basketball3x3TeamId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "Basketball3x3TeamId",
                table: "players");

            migrationBuilder.RenameColumn(
                name: "Basketball3x3MatchId",
                table: "basketball3x3_match_home_roster",
                newName: "BasketballMatchId");

            migrationBuilder.RenameColumn(
                name: "Basketball3x3MatchId",
                table: "basketball3x3_match_away_roster",
                newName: "BasketballMatchId");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
                .Annotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .Annotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .Annotation("Npgsql:Enum:basketball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .Annotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .Annotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .Annotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .Annotation("Npgsql:Enum:event_category", "men,women,mixed")
                .Annotation("Npgsql:Enum:football_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .Annotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team,message")
                .Annotation("Npgsql:Enum:registration_status", "pending,approved,rejected,ignored")
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
                .OldAnnotation("Npgsql:Enum:football_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .OldAnnotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team,message")
                .OldAnnotation("Npgsql:Enum:registration_status", "pending,approved,rejected,ignored")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "none,user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.CreateIndex(
                name: "IX_players_basketball_team_id",
                table: "players",
                column: "basketball_team_id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_basketball3x3_teams_basketball_team_id",
                table: "players",
                column: "basketball_team_id",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_basketball3x3_teams_basketball_team_id",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_basketball_team_id",
                table: "players");

            migrationBuilder.RenameColumn(
                name: "BasketballMatchId",
                table: "basketball3x3_match_home_roster",
                newName: "Basketball3x3MatchId");

            migrationBuilder.RenameColumn(
                name: "BasketballMatchId",
                table: "basketball3x3_match_away_roster",
                newName: "Basketball3x3MatchId");

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
                .Annotation("Npgsql:Enum:football_position", "goalkeeper,defender,winger,forward")
                .Annotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .Annotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team,message")
                .Annotation("Npgsql:Enum:registration_status", "pending,approved,rejected,ignored")
                .Annotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .Annotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .Annotation("Npgsql:Enum:user_role", "none,user,admin")
                .Annotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .Annotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:armwrestling_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:armwrestling_weight_class", "under60kg,under70kg,under80kg,under90kg,plus90kg")
                .OldAnnotation("Npgsql:Enum:badminton_tournament_type", "single_elimination,double_elimination,round_robin,groups_playoff,swiss")
                .OldAnnotation("Npgsql:Enum:basketball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:beach_volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff")
                .OldAnnotation("Npgsql:Enum:checkers_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:checkers_variant", "russian,international,english")
                .OldAnnotation("Npgsql:Enum:chess_system", "swiss,round_robin,knockout")
                .OldAnnotation("Npgsql:Enum:competition_level", "college_group,regional,national")
                .OldAnnotation("Npgsql:Enum:event_category", "men,women,mixed")
                .OldAnnotation("Npgsql:Enum:football_position", "goalkeeper,defender,winger,forward")
                .OldAnnotation("Npgsql:Enum:game_result", "white_win,black_win,draw,not_played")
                .OldAnnotation("Npgsql:Enum:notification_type", "system,tournament,achievement,match,team,message")
                .OldAnnotation("Npgsql:Enum:registration_status", "pending,approved,rejected,ignored")
                .OldAnnotation("Npgsql:Enum:table_tennis_hand", "left,right,both")
                .OldAnnotation("Npgsql:Enum:table_tennis_tournament_type", "single_elimination,double_elimination,round_robin")
                .OldAnnotation("Npgsql:Enum:user_role", "none,user,admin")
                .OldAnnotation("Npgsql:Enum:volleyball_position", "setter,outside_hitter,middle_blocker,opposite,libero")
                .OldAnnotation("Npgsql:Enum:volleyball_tournament_type", "single_elimination,double_elimination,round_robin,swiss,groups_playoff");

            migrationBuilder.AddColumn<long>(
                name: "Basketball3x3TeamId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_Basketball3x3TeamId",
                table: "players",
                column: "Basketball3x3TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_players_basketball3x3_teams_Basketball3x3TeamId",
                table: "players",
                column: "Basketball3x3TeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");
        }
    }
}
