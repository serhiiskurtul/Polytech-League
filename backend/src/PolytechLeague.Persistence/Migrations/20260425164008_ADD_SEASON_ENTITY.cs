using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ADD_SEASON_ENTITY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Points",
                table: "futsal_teams",
                newName: "points");

            migrationBuilder.AddColumn<int>(
                name: "rank",
                table: "volleyball_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rank",
                table: "futsal_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "season_id",
                table: "events",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "rank",
                table: "beach_volleyball_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "rank",
                table: "basketball3x3_teams",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_current = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_seasons", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_events_season_id",
                table: "events",
                column: "season_id");

            migrationBuilder.AddForeignKey(
                name: "FK_events_seasons_season_id",
                table: "events",
                column: "season_id",
                principalTable: "seasons",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_events_seasons_season_id",
                table: "events");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropIndex(
                name: "IX_events_season_id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "rank",
                table: "volleyball_teams");

            migrationBuilder.DropColumn(
                name: "rank",
                table: "futsal_teams");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "events");

            migrationBuilder.DropColumn(
                name: "rank",
                table: "beach_volleyball_teams");

            migrationBuilder.DropColumn(
                name: "rank",
                table: "basketball3x3_teams");

            migrationBuilder.RenameColumn(
                name: "points",
                table: "futsal_teams",
                newName: "Points");
        }
    }
}
