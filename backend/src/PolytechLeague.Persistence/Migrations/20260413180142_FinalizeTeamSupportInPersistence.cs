using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FinalizeTeamSupportInPersistence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_BlackTeamId",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_WhiteTeamId",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_BlackTeamId",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_WhiteTeamId",
                table: "chess_games");

            migrationBuilder.RenameColumn(
                name: "WhiteTeamScore",
                table: "chess_games",
                newName: "white_team_score");

            migrationBuilder.RenameColumn(
                name: "WhiteTeamId",
                table: "chess_games",
                newName: "white_team_id");

            migrationBuilder.RenameColumn(
                name: "BlackTeamScore",
                table: "chess_games",
                newName: "black_team_score");

            migrationBuilder.RenameColumn(
                name: "BlackTeamId",
                table: "chess_games",
                newName: "black_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_chess_games_WhiteTeamId",
                table: "chess_games",
                newName: "IX_chess_games_white_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_chess_games_BlackTeamId",
                table: "chess_games",
                newName: "IX_chess_games_black_team_id");

            migrationBuilder.RenameColumn(
                name: "WhiteTeamScore",
                table: "checkers_games",
                newName: "white_team_score");

            migrationBuilder.RenameColumn(
                name: "WhiteTeamId",
                table: "checkers_games",
                newName: "white_team_id");

            migrationBuilder.RenameColumn(
                name: "BlackTeamScore",
                table: "checkers_games",
                newName: "black_team_score");

            migrationBuilder.RenameColumn(
                name: "BlackTeamId",
                table: "checkers_games",
                newName: "black_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_games_WhiteTeamId",
                table: "checkers_games",
                newName: "IX_checkers_games_white_team_id");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_games_BlackTeamId",
                table: "checkers_games",
                newName: "IX_checkers_games_black_team_id");

            migrationBuilder.AlterColumn<long>(
                name: "white_player_id",
                table: "chess_games",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "black_player_id",
                table: "chess_games",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "white_player_id",
                table: "checkers_games",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "black_player_id",
                table: "checkers_games",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_black_team_id",
                table: "checkers_games",
                column: "black_team_id",
                principalTable: "checkers_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_white_team_id",
                table: "checkers_games",
                column: "white_team_id",
                principalTable: "checkers_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_black_team_id",
                table: "chess_games",
                column: "black_team_id",
                principalTable: "chess_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_white_team_id",
                table: "chess_games",
                column: "white_team_id",
                principalTable: "chess_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_black_team_id",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_games_checkers_teams_white_team_id",
                table: "checkers_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_black_team_id",
                table: "chess_games");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_teams_white_team_id",
                table: "chess_games");

            migrationBuilder.RenameColumn(
                name: "white_team_score",
                table: "chess_games",
                newName: "WhiteTeamScore");

            migrationBuilder.RenameColumn(
                name: "white_team_id",
                table: "chess_games",
                newName: "WhiteTeamId");

            migrationBuilder.RenameColumn(
                name: "black_team_score",
                table: "chess_games",
                newName: "BlackTeamScore");

            migrationBuilder.RenameColumn(
                name: "black_team_id",
                table: "chess_games",
                newName: "BlackTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_chess_games_white_team_id",
                table: "chess_games",
                newName: "IX_chess_games_WhiteTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_chess_games_black_team_id",
                table: "chess_games",
                newName: "IX_chess_games_BlackTeamId");

            migrationBuilder.RenameColumn(
                name: "white_team_score",
                table: "checkers_games",
                newName: "WhiteTeamScore");

            migrationBuilder.RenameColumn(
                name: "white_team_id",
                table: "checkers_games",
                newName: "WhiteTeamId");

            migrationBuilder.RenameColumn(
                name: "black_team_score",
                table: "checkers_games",
                newName: "BlackTeamScore");

            migrationBuilder.RenameColumn(
                name: "black_team_id",
                table: "checkers_games",
                newName: "BlackTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_games_white_team_id",
                table: "checkers_games",
                newName: "IX_checkers_games_WhiteTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_checkers_games_black_team_id",
                table: "checkers_games",
                newName: "IX_checkers_games_BlackTeamId");

            migrationBuilder.AlterColumn<long>(
                name: "white_player_id",
                table: "chess_games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "black_player_id",
                table: "chess_games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "white_player_id",
                table: "checkers_games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "black_player_id",
                table: "checkers_games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_BlackTeamId",
                table: "checkers_games",
                column: "BlackTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_games_checkers_teams_WhiteTeamId",
                table: "checkers_games",
                column: "WhiteTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_BlackTeamId",
                table: "chess_games",
                column: "BlackTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_teams_WhiteTeamId",
                table: "chess_games",
                column: "WhiteTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");
        }
    }
}
