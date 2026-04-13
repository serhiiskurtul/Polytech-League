using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddChessAndCheckersTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CheckersTeamId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ChessTeamId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "checkers_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_checkers_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_checkers_teams_checkers_events_EventId",
                        column: x => x.EventId,
                        principalTable: "checkers_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "chess_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chess_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_chess_teams_chess_events_EventId",
                        column: x => x.EventId,
                        principalTable: "chess_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_players_CheckersTeamId",
                table: "players",
                column: "CheckersTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_players_ChessTeamId",
                table: "players",
                column: "ChessTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_teams_EventId",
                table: "checkers_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_teams_EventId",
                table: "chess_teams",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_players_checkers_teams_CheckersTeamId",
                table: "players",
                column: "CheckersTeamId",
                principalTable: "checkers_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_chess_teams_ChessTeamId",
                table: "players",
                column: "ChessTeamId",
                principalTable: "chess_teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_players_checkers_teams_CheckersTeamId",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_players_chess_teams_ChessTeamId",
                table: "players");

            migrationBuilder.DropTable(
                name: "checkers_teams");

            migrationBuilder.DropTable(
                name: "chess_teams");

            migrationBuilder.DropIndex(
                name: "IX_players_CheckersTeamId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_ChessTeamId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "CheckersTeamId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "ChessTeamId",
                table: "players");
        }
    }
}
