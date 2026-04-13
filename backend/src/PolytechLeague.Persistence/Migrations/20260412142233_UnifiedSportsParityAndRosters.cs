using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UnifiedSportsParityAndRosters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_players_WinnerId",
                table: "badminton_matches");

            migrationBuilder.RenameColumn(
                name: "WinnerId",
                table: "badminton_matches",
                newName: "WinnerTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_WinnerId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_WinnerTeamId");

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "futsal_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "futsal_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "futsal_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "beach_volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "beach_volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "beach_volleyball_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "basketball3x3_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "basketball3x3_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "basketball3x3_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Player2Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "Player1Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "Team1Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Team2Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "WinnerPlayerId",
                table: "badminton_matches",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FirstPlaceTeamId",
                table: "badminton_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "SecondPlaceTeamId",
                table: "badminton_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ThirdPlaceTeamId",
                table: "badminton_events",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "badminton_teams",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Player1Id = table.Column<long>(type: "bigint", nullable: false),
                    Player2Id = table.Column<long>(type: "bigint", nullable: false),
                    EventId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_badminton_teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_badminton_teams_badminton_events_EventId",
                        column: x => x.EventId,
                        principalTable: "badminton_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_badminton_teams_players_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_badminton_teams_players_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_match_away_roster",
                columns: table => new
                {
                    AwayRosterId = table.Column<long>(type: "bigint", nullable: false),
                    Basketball3x3Match1Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_match_away_roster", x => new { x.AwayRosterId, x.Basketball3x3Match1Id });
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_away_roster_basketball3x3_matches_Baske~",
                        column: x => x.Basketball3x3Match1Id,
                        principalTable: "basketball3x3_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_away_roster_players_AwayRosterId",
                        column: x => x.AwayRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "basketball3x3_match_home_roster",
                columns: table => new
                {
                    Basketball3x3MatchId = table.Column<long>(type: "bigint", nullable: false),
                    HomeRosterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_basketball3x3_match_home_roster", x => new { x.Basketball3x3MatchId, x.HomeRosterId });
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_home_roster_basketball3x3_matches_Baske~",
                        column: x => x.Basketball3x3MatchId,
                        principalTable: "basketball3x3_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_basketball3x3_match_home_roster_players_HomeRosterId",
                        column: x => x.HomeRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_match_away_roster",
                columns: table => new
                {
                    AwayRosterId = table.Column<long>(type: "bigint", nullable: false),
                    BeachVolleyballMatch1Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_match_away_roster", x => new { x.AwayRosterId, x.BeachVolleyballMatch1Id });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_away_roster_beach_volleyball_matches~",
                        column: x => x.BeachVolleyballMatch1Id,
                        principalTable: "beach_volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_away_roster_players_AwayRosterId",
                        column: x => x.AwayRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "beach_volleyball_match_home_roster",
                columns: table => new
                {
                    BeachVolleyballMatchId = table.Column<long>(type: "bigint", nullable: false),
                    HomeRosterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_beach_volleyball_match_home_roster", x => new { x.BeachVolleyballMatchId, x.HomeRosterId });
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_home_roster_beach_volleyball_matches~",
                        column: x => x.BeachVolleyballMatchId,
                        principalTable: "beach_volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_beach_volleyball_match_home_roster_players_HomeRosterId",
                        column: x => x.HomeRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_match_away_roster",
                columns: table => new
                {
                    AwayRosterId = table.Column<long>(type: "bigint", nullable: false),
                    FutsalMatch1Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_match_away_roster", x => new { x.AwayRosterId, x.FutsalMatch1Id });
                    table.ForeignKey(
                        name: "FK_futsal_match_away_roster_futsal_matches_FutsalMatch1Id",
                        column: x => x.FutsalMatch1Id,
                        principalTable: "futsal_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_futsal_match_away_roster_players_AwayRosterId",
                        column: x => x.AwayRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "futsal_match_home_roster",
                columns: table => new
                {
                    FutsalMatchId = table.Column<long>(type: "bigint", nullable: false),
                    HomeRosterId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_futsal_match_home_roster", x => new { x.FutsalMatchId, x.HomeRosterId });
                    table.ForeignKey(
                        name: "FK_futsal_match_home_roster_futsal_matches_FutsalMatchId",
                        column: x => x.FutsalMatchId,
                        principalTable: "futsal_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_futsal_match_home_roster_players_HomeRosterId",
                        column: x => x.HomeRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_match_away_roster",
                columns: table => new
                {
                    AwayRosterId = table.Column<long>(type: "bigint", nullable: false),
                    VolleyballMatch1Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_match_away_roster", x => new { x.AwayRosterId, x.VolleyballMatch1Id });
                    table.ForeignKey(
                        name: "FK_volleyball_match_away_roster_players_AwayRosterId",
                        column: x => x.AwayRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_match_away_roster_volleyball_matches_VolleyballM~",
                        column: x => x.VolleyballMatch1Id,
                        principalTable: "volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "volleyball_match_home_roster",
                columns: table => new
                {
                    HomeRosterId = table.Column<long>(type: "bigint", nullable: false),
                    VolleyballMatchId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_volleyball_match_home_roster", x => new { x.HomeRosterId, x.VolleyballMatchId });
                    table.ForeignKey(
                        name: "FK_volleyball_match_home_roster_players_HomeRosterId",
                        column: x => x.HomeRosterId,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_volleyball_match_home_roster_volleyball_matches_VolleyballM~",
                        column: x => x.VolleyballMatchId,
                        principalTable: "volleyball_matches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_FirstPlaceTeamId",
                table: "volleyball_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_SecondPlaceTeamId",
                table: "volleyball_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_events_ThirdPlaceTeamId",
                table: "volleyball_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_FirstPlaceTeamId",
                table: "futsal_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_SecondPlaceTeamId",
                table: "futsal_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_events_ThirdPlaceTeamId",
                table: "futsal_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_FirstPlaceTeamId",
                table: "beach_volleyball_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_SecondPlaceTeamId",
                table: "beach_volleyball_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_events_ThirdPlaceTeamId",
                table: "beach_volleyball_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_FirstPlaceTeamId",
                table: "basketball3x3_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_SecondPlaceTeamId",
                table: "basketball3x3_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_events_ThirdPlaceTeamId",
                table: "basketball3x3_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Team1Id",
                table: "badminton_matches",
                column: "Team1Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_Team2Id",
                table: "badminton_matches",
                column: "Team2Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_matches_WinnerPlayerId",
                table: "badminton_matches",
                column: "WinnerPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_FirstPlaceTeamId",
                table: "badminton_events",
                column: "FirstPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_SecondPlaceTeamId",
                table: "badminton_events",
                column: "SecondPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_events_ThirdPlaceTeamId",
                table: "badminton_events",
                column: "ThirdPlaceTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_EventId",
                table: "badminton_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_Player1Id",
                table: "badminton_teams",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_Player2Id",
                table: "badminton_teams",
                column: "Player2Id");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_match_away_roster_Basketball3x3Match1Id",
                table: "basketball3x3_match_away_roster",
                column: "Basketball3x3Match1Id");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_match_home_roster_HomeRosterId",
                table: "basketball3x3_match_home_roster",
                column: "HomeRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_match_away_roster_BeachVolleyballMatch1Id",
                table: "beach_volleyball_match_away_roster",
                column: "BeachVolleyballMatch1Id");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_match_home_roster_HomeRosterId",
                table: "beach_volleyball_match_home_roster",
                column: "HomeRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_match_away_roster_FutsalMatch1Id",
                table: "futsal_match_away_roster",
                column: "FutsalMatch1Id");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_match_home_roster_HomeRosterId",
                table: "futsal_match_home_roster",
                column: "HomeRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_match_away_roster_VolleyballMatch1Id",
                table: "volleyball_match_away_roster",
                column: "VolleyballMatch1Id");

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_match_home_roster_VolleyballMatchId",
                table: "volleyball_match_home_roster",
                column: "VolleyballMatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_FirstPlaceTeamId",
                table: "badminton_events",
                column: "FirstPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_SecondPlaceTeamId",
                table: "badminton_events",
                column: "SecondPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_ThirdPlaceTeamId",
                table: "badminton_events",
                column: "ThirdPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team1Id",
                table: "badminton_matches",
                column: "Team1Id",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team2Id",
                table: "badminton_matches",
                column: "Team2Id",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_badminton_teams_WinnerTeamId",
                table: "badminton_matches",
                column: "WinnerTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_WinnerPlayerId",
                table: "badminton_matches",
                column: "WinnerPlayerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events",
                column: "FirstPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events",
                column: "SecondPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events",
                column: "ThirdPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_FirstPlaceTe~",
                table: "beach_volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_SecondPlaceT~",
                table: "beach_volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_ThirdPlaceTe~",
                table: "beach_volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_FirstPlaceTeamId",
                table: "futsal_events",
                column: "FirstPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_SecondPlaceTeamId",
                table: "futsal_events",
                column: "SecondPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_ThirdPlaceTeamId",
                table: "futsal_events",
                column: "ThirdPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_badminton_events_badminton_teams_FirstPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_events_badminton_teams_SecondPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_events_badminton_teams_ThirdPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team1Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_Team2Id",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_badminton_teams_WinnerTeamId",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_matches_players_WinnerPlayerId",
                table: "badminton_matches");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_FirstPlaceTe~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_SecondPlaceT~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_ThirdPlaceTe~",
                table: "beach_volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_FirstPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_SecondPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_events_futsal_teams_ThirdPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropTable(
                name: "badminton_teams");

            migrationBuilder.DropTable(
                name: "basketball3x3_match_away_roster");

            migrationBuilder.DropTable(
                name: "basketball3x3_match_home_roster");

            migrationBuilder.DropTable(
                name: "beach_volleyball_match_away_roster");

            migrationBuilder.DropTable(
                name: "beach_volleyball_match_home_roster");

            migrationBuilder.DropTable(
                name: "futsal_match_away_roster");

            migrationBuilder.DropTable(
                name: "futsal_match_home_roster");

            migrationBuilder.DropTable(
                name: "volleyball_match_away_roster");

            migrationBuilder.DropTable(
                name: "volleyball_match_home_roster");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_events_FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_events_SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_events_ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_futsal_events_FirstPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropIndex(
                name: "IX_futsal_events_SecondPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropIndex(
                name: "IX_futsal_events_ThirdPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_events_FirstPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_events_SecondPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_events_ThirdPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_events_FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_events_SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_events_ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropIndex(
                name: "IX_badminton_matches_Team1Id",
                table: "badminton_matches");

            migrationBuilder.DropIndex(
                name: "IX_badminton_matches_Team2Id",
                table: "badminton_matches");

            migrationBuilder.DropIndex(
                name: "IX_badminton_matches_WinnerPlayerId",
                table: "badminton_matches");

            migrationBuilder.DropIndex(
                name: "IX_badminton_events_FirstPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropIndex(
                name: "IX_badminton_events_SecondPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropIndex(
                name: "IX_badminton_events_ThirdPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "futsal_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "beach_volleyball_events");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropColumn(
                name: "Team1Id",
                table: "badminton_matches");

            migrationBuilder.DropColumn(
                name: "Team2Id",
                table: "badminton_matches");

            migrationBuilder.DropColumn(
                name: "WinnerPlayerId",
                table: "badminton_matches");

            migrationBuilder.DropColumn(
                name: "FirstPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropColumn(
                name: "SecondPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.DropColumn(
                name: "ThirdPlaceTeamId",
                table: "badminton_events");

            migrationBuilder.RenameColumn(
                name: "WinnerTeamId",
                table: "badminton_matches",
                newName: "WinnerId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_WinnerTeamId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_WinnerId");

            migrationBuilder.AlterColumn<long>(
                name: "Player2Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Player1Id",
                table: "badminton_matches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_matches_players_WinnerId",
                table: "badminton_matches",
                column: "WinnerId",
                principalTable: "players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
