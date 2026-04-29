using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_FUTSAL_GOALKEEPER : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "GoalkeeperId",
                table: "futsal_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_GoalkeeperId",
                table: "futsal_teams",
                column: "GoalkeeperId");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_teams_players_GoalkeeperId",
                table: "futsal_teams",
                column: "GoalkeeperId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_teams_players_GoalkeeperId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_GoalkeeperId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "GoalkeeperId",
                table: "futsal_teams");
        }
    }
}
