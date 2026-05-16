using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardRankToChessAndCheckers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "board_rank",
                table: "chess_event_participants",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "board_rank",
                table: "checkers_event_participants",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "board_rank",
                table: "chess_event_participants");

            migrationBuilder.DropColumn(
                name: "board_rank",
                table: "checkers_event_participants");
        }
    }
}
