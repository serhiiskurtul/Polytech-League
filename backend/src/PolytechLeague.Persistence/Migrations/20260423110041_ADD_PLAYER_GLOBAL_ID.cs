using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_PLAYER_GLOBAL_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "table_tennis_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentYear",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupNumber",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpecialtyCode",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "chess_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "checkers_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "badminton_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "armwrestling_event_participants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "table_tennis_event_participants");

            migrationBuilder.DropColumn(
                name: "EnrollmentYear",
                table: "players");

            migrationBuilder.DropColumn(
                name: "GroupNumber",
                table: "players");

            migrationBuilder.DropColumn(
                name: "SpecialtyCode",
                table: "players");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "chess_event_participants");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "checkers_event_participants");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "badminton_event_participants");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "armwrestling_event_participants");
        }
    }
}
