using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_TO_ARMRESTLING_PARTICIPANTS_LIST : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "armwrestling_event_participants",
                columns: table => new
                {
                    TournamentId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_armwrestling_event_participants", x => new { x.TournamentId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_armwrestling_event_participants_armwrestling_events_Tournam~",
                        column: x => x.TournamentId,
                        principalTable: "armwrestling_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_armwrestling_event_participants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_event_participants_PlayerId",
                table: "armwrestling_event_participants",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "armwrestling_event_participants");
        }
    }
}
