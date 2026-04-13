using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_CHESS_AND_CHECKERS_TEAM_WINNERS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "chess_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "chess_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "chess_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "checkers_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "checkers_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "checkers_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_FirstPlaceTeamId",
                table: "chess_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_SecondPlaceTeamId",
                table: "chess_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_events_ThirdPlaceTeamId",
                table: "chess_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_FirstPlaceTeamId",
                table: "checkers_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_SecondPlaceTeamId",
                table: "checkers_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_events_ThirdPlaceTeamId",
                table: "checkers_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_FirstPlaceTeamId",
                table: "checkers_events",
                column: "FirstPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_SecondPlaceTeamId",
                table: "checkers_events",
                column: "SecondPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_events_checkers_teams_ThirdPlaceTeamId",
                table: "checkers_events",
                column: "ThirdPlaceTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_FirstPlaceTeamId",
                table: "chess_events",
                column: "FirstPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_SecondPlaceTeamId",
                table: "chess_events",
                column: "SecondPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_events_chess_teams_ThirdPlaceTeamId",
                table: "chess_events",
                column: "ThirdPlaceTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_checkers_events_checkers_teams_FirstPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_events_checkers_teams_SecondPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_events_checkers_teams_ThirdPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_events_chess_teams_FirstPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_events_chess_teams_SecondPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_events_chess_teams_ThirdPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropIndex(
                name: "IX_chess_events_FirstPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropIndex(
                name: "IX_chess_events_SecondPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropIndex(
                name: "IX_chess_events_ThirdPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropIndex(
                name: "IX_checkers_events_FirstPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropIndex(
                name: "IX_checkers_events_SecondPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropIndex(
                name: "IX_checkers_events_ThirdPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "chess_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "checkers_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "checkers_events");
        }
    }
}
