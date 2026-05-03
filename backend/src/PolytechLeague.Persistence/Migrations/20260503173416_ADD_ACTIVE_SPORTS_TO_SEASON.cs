using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_ACTIVE_SPORTS_TO_SEASON : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NationalRatingSports",
                table: "seasons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegionalRatingSports",
                table: "seasons",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NationalRatingSports",
                table: "seasons");

            migrationBuilder.DropColumn(
                name: "RegionalRatingSports",
                table: "seasons");
        }
    }
}
