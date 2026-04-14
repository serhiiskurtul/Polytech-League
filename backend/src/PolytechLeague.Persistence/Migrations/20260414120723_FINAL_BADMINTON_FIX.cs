using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FINAL_BADMINTON_FIX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // SWAP COLUMNS BACK
            migrationBuilder.RenameColumn(
                name: "HomeTeamId",
                table: "badminton_matches",
                newName: "TempTeamId");

            migrationBuilder.RenameColumn(
                name: "AwayTeamId",
                table: "badminton_matches",
                newName: "HomeTeamId");

            migrationBuilder.RenameColumn(
                name: "TempTeamId",
                table: "badminton_matches",
                newName: "AwayTeamId");

            // RENAME INDEXES
            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_HomeTeamId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_AwayTeamId_Temp");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_AwayTeamId",
                table: "badminton_matches",
                newName: "IX_badminton_matches_HomeTeamId");

            migrationBuilder.RenameIndex(
                name: "IX_badminton_matches_AwayTeamId_Temp",
                table: "badminton_matches",
                newName: "IX_badminton_matches_AwayTeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AwayTeamId",
                table: "badminton_matches",
                newName: "TempTeamId");

            migrationBuilder.RenameColumn(
                name: "HomeTeamId",
                table: "badminton_matches",
                newName: "AwayTeamId");

            migrationBuilder.RenameColumn(
                name: "TempTeamId",
                table: "badminton_matches",
                newName: "HomeTeamId");
        }
    }
}
