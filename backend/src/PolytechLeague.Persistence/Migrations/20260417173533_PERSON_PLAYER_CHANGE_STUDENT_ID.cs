using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PERSON_PLAYER_CHANGE_STUDENT_ID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "student_id",
                table: "players",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Patronymic",
                table: "persons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "armwrestling_event_participants",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "armwrestling_event_participants",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_players_student_id",
                table: "players",
                column: "student_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_players_student_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "student_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "Patronymic",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "armwrestling_event_participants");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "armwrestling_event_participants");
        }
    }
}
