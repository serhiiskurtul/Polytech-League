using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ModarationCommandSportByAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "basketball3x3_event_participants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_event_participants", x => new { x.EventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_basketball3x3_event_participants_basketball3x3_events_Event~",
                        column: x => x.EventId,
                        principalTable: "basketball3x3_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball3x3_event_participants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_event_participants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_event_participants", x => new { x.EventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_event_participants_beach_volleyball_events~",
                        column: x => x.EventId,
                        principalTable: "beach_volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_event_participants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_event_participants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_event_participants", x => new { x.EventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_futsal_event_participants_futsal_events_EventId",
                        column: x => x.EventId,
                        principalTable: "futsal_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_futsal_event_participants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_event_participants",
                columns: table => new
                {
                    EventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_event_participants", x => new { x.EventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_volleyball_event_participants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_event_participants_volleyball_events_EventId",
                        column: x => x.EventId,
                        principalTable: "volleyball_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_event_participants_PlayerId",
                table: "basketball3x3_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_event_participants_PlayerId",
                table: "beach_volleyball_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_event_participants_PlayerId",
                table: "futsal_event_participants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_event_participants_PlayerId",
                table: "volleyball_event_participants",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basketball3x3_event_participants");

            migrationBuilder.DropTable(
                name: "beach_volleyball_event_participants");

            migrationBuilder.DropTable(
                name: "futsal_event_participants");

            migrationBuilder.DropTable(
                name: "volleyball_event_participants");
        }
    }
}
