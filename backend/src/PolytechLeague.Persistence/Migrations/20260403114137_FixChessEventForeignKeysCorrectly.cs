using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixChessEventForeignKeysCorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games");

            migrationBuilder.DropIndex(
                name: "IX_chess_games_ChessEventId",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "ChessEventId",
                table: "chess_games");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ChessEventId",
                table: "chess_games",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_chess_games_ChessEventId",
                table: "chess_games",
                column: "ChessEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games",
                column: "ChessEventId",
                principalTable: "chess_events",
                principalColumn: "id");
        }
    }
}
