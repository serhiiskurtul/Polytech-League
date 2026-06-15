using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPointsRankToChessCheckersTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "chess_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "chess_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "chess_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "chess_teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "checkers_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "checkers_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "checkers_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "checkers_teams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "chess_teams");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "chess_teams");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "chess_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "chess_teams");

            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "checkers_teams");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "checkers_teams");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "checkers_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "checkers_teams");
        }
    }
}
