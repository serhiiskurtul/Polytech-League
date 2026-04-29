using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_FUTSAL_CAPTAIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CaptainId",
                table: "futsal_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_CaptainId",
                table: "futsal_teams",
                column: "CaptainId");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_teams_players_CaptainId",
                table: "futsal_teams",
                column: "CaptainId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_futsal_teams_players_CaptainId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_CaptainId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "CaptainId",
                table: "futsal_teams");
        }
    }
}
