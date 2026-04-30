using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCollegesAndNationalRatings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "table_tennis_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "table_tennis_event_participants",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "players",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "futsal_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "chess_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "chess_event_participants",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "checkers_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "checkers_event_participants",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "beach_volleyball_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "basketball3x3_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "badminton_teams",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "badminton_event_participants",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CollegeId",
                table: "armwrestling_event_participants",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Colleges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colleges", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_volleyball_teams_CollegeId",
                table: "volleyball_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_teams_CollegeId",
                table: "table_tennis_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_table_tennis_event_participants_CollegeId",
                table: "table_tennis_event_participants",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_players_CollegeId",
                table: "players",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_futsal_teams_CollegeId",
                table: "futsal_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_teams_CollegeId",
                table: "chess_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_chess_event_participants_CollegeId",
                table: "chess_event_participants",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_teams_CollegeId",
                table: "checkers_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_checkers_event_participants_CollegeId",
                table: "checkers_event_participants",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_beach_volleyball_teams_CollegeId",
                table: "beach_volleyball_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_basketball3x3_teams_CollegeId",
                table: "basketball3x3_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_teams_CollegeId",
                table: "badminton_teams",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_badminton_event_participants_CollegeId",
                table: "badminton_event_participants",
                column: "CollegeId");

            migrationBuilder.CreateIndex(
                name: "IX_armwrestling_event_participants_CollegeId",
                table: "armwrestling_event_participants",
                column: "CollegeId");

            migrationBuilder.AddForeignKey(
                name: "FK_armwrestling_event_participants_Colleges_CollegeId",
                table: "armwrestling_event_participants",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_event_participants_Colleges_CollegeId",
                table: "badminton_event_participants",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_badminton_teams_Colleges_CollegeId",
                table: "badminton_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_basketball3x3_teams_Colleges_CollegeId",
                table: "basketball3x3_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_beach_volleyball_teams_Colleges_CollegeId",
                table: "beach_volleyball_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_event_participants_Colleges_CollegeId",
                table: "checkers_event_participants",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_checkers_teams_Colleges_CollegeId",
                table: "checkers_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_event_participants_Colleges_CollegeId",
                table: "chess_event_participants",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chess_teams_Colleges_CollegeId",
                table: "chess_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_futsal_teams_Colleges_CollegeId",
                table: "futsal_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_players_Colleges_CollegeId",
                table: "players",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_event_participants_Colleges_CollegeId",
                table: "table_tennis_event_participants",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_table_tennis_teams_Colleges_CollegeId",
                table: "table_tennis_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_volleyball_teams_Colleges_CollegeId",
                table: "volleyball_teams",
                column: "CollegeId",
                principalTable: "Colleges",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_armwrestling_event_participants_Colleges_CollegeId",
                table: "armwrestling_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_event_participants_Colleges_CollegeId",
                table: "badminton_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_badminton_teams_Colleges_CollegeId",
                table: "badminton_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_basketball3x3_teams_Colleges_CollegeId",
                table: "basketball3x3_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_beach_volleyball_teams_Colleges_CollegeId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_event_participants_Colleges_CollegeId",
                table: "checkers_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_checkers_teams_Colleges_CollegeId",
                table: "checkers_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_event_participants_Colleges_CollegeId",
                table: "chess_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_chess_teams_Colleges_CollegeId",
                table: "chess_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_futsal_teams_Colleges_CollegeId",
                table: "futsal_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_players_Colleges_CollegeId",
                table: "players");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_event_participants_Colleges_CollegeId",
                table: "table_tennis_event_participants");

            migrationBuilder.DropForeignKey(
                name: "FK_table_tennis_teams_Colleges_CollegeId",
                table: "table_tennis_teams");

            migrationBuilder.DropForeignKey(
                name: "FK_volleyball_teams_Colleges_CollegeId",
                table: "volleyball_teams");

            migrationBuilder.DropTable(
                name: "Colleges");

            migrationBuilder.DropIndex(
                name: "IX_volleyball_teams_CollegeId",
                table: "volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_teams_CollegeId",
                table: "table_tennis_teams");

            migrationBuilder.DropIndex(
                name: "IX_table_tennis_event_participants_CollegeId",
                table: "table_tennis_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_players_CollegeId",
                table: "players");

            migrationBuilder.DropIndex(
                name: "IX_futsal_teams_CollegeId",
                table: "futsal_teams");

            migrationBuilder.DropIndex(
                name: "IX_chess_teams_CollegeId",
                table: "chess_teams");

            migrationBuilder.DropIndex(
                name: "IX_chess_event_participants_CollegeId",
                table: "chess_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_checkers_teams_CollegeId",
                table: "checkers_teams");

            migrationBuilder.DropIndex(
                name: "IX_checkers_event_participants_CollegeId",
                table: "checkers_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_beach_volleyball_teams_CollegeId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropIndex(
                name: "IX_basketball3x3_teams_CollegeId",
                table: "basketball3x3_teams");

            migrationBuilder.DropIndex(
                name: "IX_badminton_teams_CollegeId",
                table: "badminton_teams");

            migrationBuilder.DropIndex(
                name: "IX_badminton_event_participants_CollegeId",
                table: "badminton_event_participants");

            migrationBuilder.DropIndex(
                name: "IX_armwrestling_event_participants_CollegeId",
                table: "armwrestling_event_participants");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "table_tennis_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "table_tennis_event_participants");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "players");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "chess_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "chess_event_participants");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "checkers_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "checkers_event_participants");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "basketball3x3_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "badminton_teams");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "badminton_event_participants");

            migrationBuilder.DropColumn(
                name: "CollegeId",
                table: "armwrestling_event_participants");
        }
    }
}
