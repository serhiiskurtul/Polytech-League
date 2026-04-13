using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_PLAYERS_ID_FUTSAL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                table: "basketball3x3_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_away_roster_players_AwayRosterId",
                table: "basketball3x3_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_home_roster_players_HomeRosterId",
                table: "basketball3x3_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                table: "beach_volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_away_roster_players_AwayRosterId",
                table: "beach_volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_home_roster_players_HomeRosterId",
                table: "beach_volleyball_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatch1Id",
                table: "futsal_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_away_roster_players_AwayRosterId",
                table: "futsal_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_home_roster_players_HomeRosterId",
                table: "futsal_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId1",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_match_away_roster_players_AwayRosterId",
                table: "volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_match_home_roster_players_HomeRosterId",
                table: "volleyball_match_home_roster");

            migrationBuilder.DropIndex(
                name: "IX_players_BadmintonMatchId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_players_BadmintonMatchId1",
                table: "players");

            migrationBuilder.DropColumn(
                name: "BadmintonMatchId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "BadmintonMatchId1",
                table: "players");

            migrationBuilder.RenameColumn(
                name: "HomeRosterId",
                table: "volleyball_match_home_roster",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "VolleyballMatch1Id",
                table: "volleyball_match_away_roster",
                newName: "VolleyballMatchId");

            migrationBuilder.RenameColumn(
                name: "AwayRosterId",
                table: "volleyball_match_away_roster",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_volleyball_match_away_roster_VolleyballMatch1Id",
                table: "volleyball_match_away_roster",
                newName: "IX_volleyball_match_away_roster_VolleyballMatchId");

            migrationBuilder.RenameColumn(
                name: "HomeRosterId",
                table: "futsal_match_home_roster",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_match_home_roster_HomeRosterId",
                table: "futsal_match_home_roster",
                newName: "IX_futsal_match_home_roster_PlayerId");

            migrationBuilder.RenameColumn(
                name: "FutsalMatch1Id",
                table: "futsal_match_away_roster",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "AwayRosterId",
                table: "futsal_match_away_roster",
                newName: "FutsalMatchId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_match_away_roster_FutsalMatch1Id",
                table: "futsal_match_away_roster",
                newName: "IX_futsal_match_away_roster_PlayerId");

            migrationBuilder.RenameColumn(
                name: "HomeRosterId",
                table: "beach_volleyball_match_home_roster",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_beach_volleyball_match_home_roster_HomeRosterId",
                table: "beach_volleyball_match_home_roster",
                newName: "IX_beach_volleyball_match_home_roster_PlayerId");

            migrationBuilder.RenameColumn(
                name: "BeachVolleyballMatch1Id",
                table: "beach_volleyball_match_away_roster",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "AwayRosterId",
                table: "beach_volleyball_match_away_roster",
                newName: "BeachVolleyballMatchId");

            migrationBuilder.RenameIndex(
                name: "IX_beach_volleyball_match_away_roster_BeachVolleyballMatch1Id",
                table: "beach_volleyball_match_away_roster",
                newName: "IX_beach_volleyball_match_away_roster_PlayerId");

            migrationBuilder.RenameColumn(
                name: "HomeRosterId",
                table: "basketball3x3_match_home_roster",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_basketball3x3_match_home_roster_HomeRosterId",
                table: "basketball3x3_match_home_roster",
                newName: "IX_basketball3x3_match_home_roster_PlayerId");

            migrationBuilder.RenameColumn(
                name: "Basketball3x3Match1Id",
                table: "basketball3x3_match_away_roster",
                newName: "PlayerId");

            migrationBuilder.RenameColumn(
                name: "AwayRosterId",
                table: "basketball3x3_match_away_roster",
                newName: "Basketball3x3MatchId");

            migrationBuilder.RenameIndex(
                name: "IX_basketball3x3_match_away_roster_Basketball3x3Match1Id",
                table: "basketball3x3_match_away_roster",
                newName: "IX_basketball3x3_match_away_roster_PlayerId");

            migrationBuilder.CreateTable(
                name: "badminton_match_away_roster",
                columns: table => new
                {
                    BadmintonMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_match_away_roster", x => new { x.BadmintonMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_badminton_match_away_roster_badminton_matches_BadmintonMatc~",
                        column: x => x.BadmintonMatchId,
                        principalTable: "badminton_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_badminton_match_away_roster_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "badminton_match_home_roster",
                columns: table => new
                {
                    BadmintonMatchId = table.Column<long>(type: "bigint", nullable: false),
                    PlayerId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_match_home_roster", x => new { x.BadmintonMatchId, x.PlayerId });
                    table.ForeignKey(
                        name: "FK_badminton_match_home_roster_badminton_matches_BadmintonMatc~",
                        column: x => x.BadmintonMatchId,
                        principalTable: "badminton_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_badminton_match_home_roster_players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_badminton_match_away_roster_PlayerId",
                table: "badminton_match_away_roster",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_match_home_roster_PlayerId",
                table: "badminton_match_home_roster",
                column: "PlayerId");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                table: "basketball3x3_match_away_roster",
                column: "Basketball3x3MatchId",
                principalTable: "basketball3x3_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_away_roster_players_PlayerId",
                table: "basketball3x3_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_home_roster_players_PlayerId",
                table: "basketball3x3_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                table: "beach_volleyball_match_away_roster",
                column: "BeachVolleyballMatchId",
                principalTable: "beach_volleyball_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_away_roster_players_PlayerId",
                table: "beach_volleyball_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_home_roster_players_PlayerId",
                table: "beach_volleyball_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatchId",
                table: "futsal_match_away_roster",
                column: "FutsalMatchId",
                principalTable: "futsal_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_away_roster_players_PlayerId",
                table: "futsal_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_home_roster_players_PlayerId",
                table: "futsal_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_match_away_roster_players_PlayerId",
                table: "volleyball_match_away_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_match_home_roster_players_PlayerId",
                table: "volleyball_match_home_roster",
                column: "PlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                table: "basketball3x3_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_away_roster_players_PlayerId",
                table: "basketball3x3_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_match_home_roster_players_PlayerId",
                table: "basketball3x3_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                table: "beach_volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_away_roster_players_PlayerId",
                table: "beach_volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_match_home_roster_players_PlayerId",
                table: "beach_volleyball_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatchId",
                table: "futsal_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_away_roster_players_PlayerId",
                table: "futsal_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_match_home_roster_players_PlayerId",
                table: "futsal_match_home_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_match_away_roster_players_PlayerId",
                table: "volleyball_match_away_roster");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_match_home_roster_players_PlayerId",
                table: "volleyball_match_home_roster");

            migrationBuilder.DropTable(
                name: "badminton_match_away_roster");

            migrationBuilder.DropTable(
                name: "badminton_match_home_roster");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "volleyball_match_home_roster",
                newName: "HomeRosterId");

            migrationBuilder.RenameColumn(
                name: "VolleyballMatchId",
                table: "volleyball_match_away_roster",
                newName: "VolleyballMatch1Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "volleyball_match_away_roster",
                newName: "AwayRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_volleyball_match_away_roster_VolleyballMatchId",
                table: "volleyball_match_away_roster",
                newName: "IX_volleyball_match_away_roster_VolleyballMatch1Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "futsal_match_home_roster",
                newName: "HomeRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_match_home_roster_PlayerId",
                table: "futsal_match_home_roster",
                newName: "IX_futsal_match_home_roster_HomeRosterId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "futsal_match_away_roster",
                newName: "FutsalMatch1Id");

            migrationBuilder.RenameColumn(
                name: "FutsalMatchId",
                table: "futsal_match_away_roster",
                newName: "AwayRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_futsal_match_away_roster_PlayerId",
                table: "futsal_match_away_roster",
                newName: "IX_futsal_match_away_roster_FutsalMatch1Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "beach_volleyball_match_home_roster",
                newName: "HomeRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_beach_volleyball_match_home_roster_PlayerId",
                table: "beach_volleyball_match_home_roster",
                newName: "IX_beach_volleyball_match_home_roster_HomeRosterId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "beach_volleyball_match_away_roster",
                newName: "BeachVolleyballMatch1Id");

            migrationBuilder.RenameColumn(
                name: "BeachVolleyballMatchId",
                table: "beach_volleyball_match_away_roster",
                newName: "AwayRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_beach_volleyball_match_away_roster_PlayerId",
                table: "beach_volleyball_match_away_roster",
                newName: "IX_beach_volleyball_match_away_roster_BeachVolleyballMatch1Id");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "basketball3x3_match_home_roster",
                newName: "HomeRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_basketball3x3_match_home_roster_PlayerId",
                table: "basketball3x3_match_home_roster",
                newName: "IX_basketball3x3_match_home_roster_HomeRosterId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "basketball3x3_match_away_roster",
                newName: "Basketball3x3Match1Id");

            migrationBuilder.RenameColumn(
                name: "Basketball3x3MatchId",
                table: "basketball3x3_match_away_roster",
                newName: "AwayRosterId");

            migrationBuilder.RenameIndex(
                name: "IX_basketball3x3_match_away_roster_PlayerId",
                table: "basketball3x3_match_away_roster",
                newName: "IX_basketball3x3_match_away_roster_Basketball3x3Match1Id");

            migrationBuilder.AddColumn<long>(
                name: "BadmintonMatchId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BadmintonMatchId1",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_players_BadmintonMatchId",
                table: "players",
                column: "BadmintonMatchId");

            migrationBuilder.CreateIndex(
                name: "IX_players_BadmintonMatchId1",
                table: "players",
                column: "BadmintonMatchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                table: "basketball3x3_match_away_roster",
                column: "Basketball3x3Match1Id",
                principalTable: "basketball3x3_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_away_roster_players_AwayRosterId",
                table: "basketball3x3_match_away_roster",
                column: "AwayRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_match_home_roster_players_HomeRosterId",
                table: "basketball3x3_match_home_roster",
                column: "HomeRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                table: "beach_volleyball_match_away_roster",
                column: "BeachVolleyballMatch1Id",
                principalTable: "beach_volleyball_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_away_roster_players_AwayRosterId",
                table: "beach_volleyball_match_away_roster",
                column: "AwayRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_match_home_roster_players_HomeRosterId",
                table: "beach_volleyball_match_home_roster",
                column: "HomeRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatch1Id",
                table: "futsal_match_away_roster",
                column: "FutsalMatch1Id",
                principalTable: "futsal_matches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_away_roster_players_AwayRosterId",
                table: "futsal_match_away_roster",
                column: "AwayRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_match_home_roster_players_HomeRosterId",
                table: "futsal_match_home_roster",
                column: "HomeRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId",
                table: "players",
                column: "BadmintonMatchId",
                principalTable: "badminton_matches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_players_badminton_matches_BadmintonMatchId1",
                table: "players",
                column: "BadmintonMatchId1",
                principalTable: "badminton_matches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_match_away_roster_players_AwayRosterId",
                table: "volleyball_match_away_roster",
                column: "AwayRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_match_home_roster_players_HomeRosterId",
                table: "volleyball_match_home_roster",
                column: "HomeRosterId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
