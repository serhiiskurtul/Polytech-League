using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoricalParticipantData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerFirstName",
                table: "table_tennis_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerLastName",
                table: "table_tennis_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerFirstName",
                table: "chess_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerLastName",
                table: "chess_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerFirstName",
                table: "checkers_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerLastName",
                table: "checkers_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerFirstName",
                table: "badminton_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerLastName",
                table: "badminton_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerFirstName",
                table: "armwrestling_event_participants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerLastName",
                table: "armwrestling_event_participants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerFirstName",
                table: "table_tennis_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerLastName",
                table: "table_tennis_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerFirstName",
                table: "chess_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerLastName",
                table: "chess_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerFirstName",
                table: "checkers_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerLastName",
                table: "checkers_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerFirstName",
                table: "badminton_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerLastName",
                table: "badminton_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerFirstName",
                table: "armwrestling_event_participants");

            migrationBuilder.DropColumn(
                name: "PlayerLastName",
                table: "armwrestling_event_participants");
        }
    }
}
