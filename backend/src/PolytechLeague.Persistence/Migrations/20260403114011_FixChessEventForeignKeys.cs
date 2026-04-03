using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixChessEventForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games");

            migrationBuilder.DropColumn(
                name: "ChessEventId",
                table: "chess_event_participants");

            migrationBuilder.AlterColumn<long>(
                name: "ChessEventId",
                table: "chess_games",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games",
                column: "ChessEventId",
                principalTable: "chess_events",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games");

            migrationBuilder.AlterColumn<long>(
                name: "ChessEventId",
                table: "chess_games",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChessEventId",
                table: "chess_event_participants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_chess_games_chess_events_ChessEventId",
                table: "chess_games",
                column: "ChessEventId",
                principalTable: "chess_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
