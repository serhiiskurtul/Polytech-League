using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CREATE_ATHLETIC_ENTITY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AthleticsEvents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleticsEvents", x => x.id);
                    table.ForeignKey(
                        name: "FK_AthleticsEvents_events_id",
                        column: x => x.id,
                        principalTable: "events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AthleticsEventParticipants",
                columns: table => new
                {
                    AthleticsEventId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false),
                    GroupName = table.Column<string>(type: "text", nullable: true),
                    PlayerFirstName = table.Column<string>(type: "text", nullable: true),
                    PlayerLastName = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CollegeId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleticsEventParticipants", x => new { x.AthleticsEventId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_AthleticsEventParticipants_AthleticsEvents_AthleticsEventId",
                        column: x => x.AthleticsEventId,
                        principalTable: "AthleticsEvents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AthleticsEventParticipants_Colleges_CollegeId",
                        column: x => x.CollegeId,
                        principalTable: "Colleges",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AthleticsEventParticipants_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AthleticsResults",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AthleticsEventId = table.Column<long>(type: "bigint", nullable: false),
                    DisciplineName = table.Column<string>(type: "text", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: true),
                    TeamName = table.Column<string>(type: "text", nullable: true),
                    Score = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AthleticsResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AthleticsResults_AthleticsEvents_AthleticsEventId",
                        column: x => x.AthleticsEventId,
                        principalTable: "AthleticsEvents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AthleticsResults_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AthleticsEventParticipants_CollegeId",
                table: "AthleticsEventParticipants",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleticsEventParticipants_PlayerId",
                table: "AthleticsEventParticipants",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleticsResults_AthleticsEventId",
                table: "AthleticsResults",
                column: "AthleticsEventId");

            migrationBuilder.CreateIndex(
                name: "IX_AthleticsResults_PlayerId",
                table: "AthleticsResults",
                column: "PlayerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AthleticsEventParticipants");

            migrationBuilder.DropTable(
                name: "AthleticsResults");

            migrationBuilder.DropTable(
                name: "AthleticsEvents");
        }
    }
}
