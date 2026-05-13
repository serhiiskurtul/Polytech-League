using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMVPComandSports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "MvpId",
                table: "events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_events_MvpId",
                table: "events",
                column: "MvpId");

            migrationBuilder.AddForeignKey(
                name: "FK_events_players_MvpId",
                table: "events",
                column: "MvpId",
                principalTable: "players",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_players_MvpId",
                table: "events");

            migrationBuilder.DropIndex(
                name: "IX_events_MvpId",
                table: "events");

            migrationBuilder.DropColumn(
                name: "MvpId",
                table: "events");
        }
    }
}
