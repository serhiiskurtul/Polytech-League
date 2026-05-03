using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_SPORT_RESULTS_TO_MANUAL_RATING : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SportResults",
                table: "RegionalRatingManualEntries",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SportResults",
                table: "NationalRatingManualEntries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SportResults",
                table: "RegionalRatingManualEntries");

            migrationBuilder.DropColumn(
                name: "SportResults",
                table: "NationalRatingManualEntries");
        }
    }
}
