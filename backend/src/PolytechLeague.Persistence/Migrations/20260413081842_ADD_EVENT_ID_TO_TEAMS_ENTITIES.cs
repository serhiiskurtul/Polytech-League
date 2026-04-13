using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_EVENT_ID_TO_TEAMS_ENTITIES : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM futsal_teams;");
            migrationBuilder.Sql("DELETE FROM basketball3x3_teams;");
            migrationBuilder.Sql("DELETE FROM volleyball_teams;");
            migrationBuilder.Sql("DELETE FROM beach_volleyball_teams;");

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
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_teams_basketball3x3_events_Basketball3x3Event~",
                table: "basketball3x3_teams");

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
                name: "FK_beach_volleyball_teams_beach_volleyball_events_BeachVolleyb~",
                table: "beach_volleyball_teams");

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
                name: "FK_futsal_teams_futsal_events_FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_teams_volleyball_events_VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_teams_VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_teams_BeachVolleyballEventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_teams_Basketball3x3EventId",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "VolleyballEventId",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "FutsalEventId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "BeachVolleyballEventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "Basketball3x3EventId",
                table: "basketball3x3_teams");

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "volleyball_teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "futsal_teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "beach_volleyball_teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "EventId",
                table: "basketball3x3_teams",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_teams_EventId",
                table: "volleyball_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_EventId",
                table: "futsal_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_teams_EventId",
                table: "beach_volleyball_teams",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_teams_EventId",
                table: "basketball3x3_teams",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_FirstPlaceTeamId",
                table: "badminton_events",
                column: "FirstPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_SecondPlaceTeamId",
                table: "badminton_events",
                column: "SecondPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_events_badminton_teams_ThirdPlaceTeamId",
                table: "badminton_events",
                column: "ThirdPlaceTeamId",
                principalTable: "badminton_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events",
                column: "FirstPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events",
                column: "SecondPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events",
                column: "ThirdPlaceTeamId",
                principalTable: "basketball3x3_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_teams_basketball3x3_events_EventId",
                table: "basketball3x3_teams",
                column: "EventId",
                principalTable: "basketball3x3_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_FirstPlaceTe~",
                table: "beach_volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_SecondPlaceT~",
                table: "beach_volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_events_beach_volleyball_teams_ThirdPlaceTe~",
                table: "beach_volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "beach_volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_teams_beach_volleyball_events_EventId",
                table: "beach_volleyball_teams",
                column: "EventId",
                principalTable: "beach_volleyball_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_FirstPlaceTeamId",
                table: "futsal_events",
                column: "FirstPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_SecondPlaceTeamId",
                table: "futsal_events",
                column: "SecondPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_events_futsal_teams_ThirdPlaceTeamId",
                table: "futsal_events",
                column: "ThirdPlaceTeamId",
                principalTable: "futsal_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_teams_futsal_events_EventId",
                table: "futsal_teams",
                column: "EventId",
                principalTable: "futsal_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events",
                column: "FirstPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events",
                column: "SecondPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events",
                column: "ThirdPlaceTeamId",
                principalTable: "volleyball_teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_teams_volleyball_events_EventId",
                table: "volleyball_teams",
                column: "EventId",
                principalTable: "volleyball_events",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
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
                name: "FK_basketball3x3_events_basketball3x3_teams_FirstPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_SecondPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_events_basketball3x3_teams_ThirdPlaceTeamId",
                table: "basketball3x3_events");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_teams_basketball3x3_events_EventId",
                table: "basketball3x3_teams");

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
                name: "FK_beach_volleyball_teams_beach_volleyball_events_EventId",
                table: "beach_volleyball_teams");

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
                name: "FK_futsal_teams_futsal_events_EventId",
                table: "futsal_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_FirstPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_SecondPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_events_volleyball_teams_ThirdPlaceTeamId",
                table: "volleyball_events");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_teams_volleyball_events_EventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_teams_EventId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_EventId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_teams_EventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_teams_EventId",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "basketball3x3_teams");

            migrationBuilder.AddColumn<long>(
                name: "VolleyballEventId",
                table: "volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FutsalEventId",
                table: "futsal_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BeachVolleyballEventId",
                table: "beach_volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Basketball3x3EventId",
                table: "basketball3x3_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_teams_VolleyballEventId",
                table: "volleyball_teams",
                column: "VolleyballEventId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_FutsalEventId",
                table: "futsal_teams",
                column: "FutsalEventId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_teams_BeachVolleyballEventId",
                table: "beach_volleyball_teams",
                column: "BeachVolleyballEventId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_teams_Basketball3x3EventId",
                table: "basketball3x3_teams",
                column: "Basketball3x3EventId");

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
                name: "FK_basketball3x3_teams_basketball3x3_events_Basketball3x3Event~",
                table: "basketball3x3_teams",
                column: "Basketball3x3EventId",
                principalTable: "basketball3x3_events",
                principalColumn: "id");

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
                name: "FK_beach_volleyball_teams_beach_volleyball_events_BeachVolleyb~",
                table: "beach_volleyball_teams",
                column: "BeachVolleyballEventId",
                principalTable: "beach_volleyball_events",
                principalColumn: "id");

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
                name: "FK_futsal_teams_futsal_events_FutsalEventId",
                table: "futsal_teams",
                column: "FutsalEventId",
                principalTable: "futsal_events",
                principalColumn: "id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_teams_volleyball_events_VolleyballEventId",
                table: "volleyball_teams",
                column: "VolleyballEventId",
                principalTable: "volleyball_events",
                principalColumn: "id");
        }
    }
}
