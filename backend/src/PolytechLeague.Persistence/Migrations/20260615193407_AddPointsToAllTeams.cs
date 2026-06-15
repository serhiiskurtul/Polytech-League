using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPointsToAllTeams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "volleyball_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "points",
                table: "table_tennis_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "table_tennis_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "table_tennis_teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "beach_volleyball_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "beach_volleyball_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "beach_volleyball_teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "basketball3x3_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "basketball3x3_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "basketball3x3_teams",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGeneralTableIncluded",
                table: "badminton_teams",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "Points",
                table: "badminton_teams",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "badminton_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TournamentGroupName",
                table: "badminton_teams",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Points",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "table_tennis_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "table_tennis_teams");

            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "IsGeneralTableIncluded",
                table: "badminton_teams");

            migrationBuilder.DropColumn(
                name: "Points",
                table: "badminton_teams");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "badminton_teams");

            migrationBuilder.DropColumn(
                name: "TournamentGroupName",
                table: "badminton_teams");

            migrationBuilder.AlterColumn<int>(
                name: "points",
                table: "table_tennis_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldDefaultValue: 0.0);
        }
    }
}
