using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CHESS_AND_CHECKERS_EVENT_SOME_CHANGES_TO_TOP_3_PLAYERS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "FirstPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceId",
                table: "armwrestling_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceId",
                table: "armwrestling_events");

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceId",
                table: "events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceId",
                table: "events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceId",
                table: "events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_FirstPlaceId",
                table: "events",
                column: "FirstPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_events_SecondPlaceId",
                table: "events",
                column: "SecondPlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_events_ThirdPlaceId",
                table: "events",
                column: "ThirdPlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_FirstPlaceId",
                table: "events",
                column: "FirstPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_SecondPlaceId",
                table: "events",
                column: "SecondPlaceId",
                principalTable: "players",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_ThirdPlaceId",
                table: "events",
                column: "ThirdPlaceId",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_players_FirstPlaceId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_players_SecondPlaceId",
                table: "events");

            migrationBuilder.DropForeignKey(
                name: "FK_events_players_ThirdPlaceId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_FirstPlaceId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_SecondPlaceId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_ThirdPlaceId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceId",
                table: "events");

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
    }
}
