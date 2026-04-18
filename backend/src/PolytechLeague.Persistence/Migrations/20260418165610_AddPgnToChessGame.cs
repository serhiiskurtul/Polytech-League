using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPgnToChessGame : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "pgn",
                table: "chess_games",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pgn",
                table: "chess_games");
        }
    }
}
