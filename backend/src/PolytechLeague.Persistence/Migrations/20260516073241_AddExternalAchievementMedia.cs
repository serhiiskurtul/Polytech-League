using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalAchievementMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "ExternalAchievements");

            migrationBuilder.CreateTable(
                name: "ExternalAchievementMedias",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    ExternalAchievementId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAchievementMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAchievementMedias_ExternalAchievements_ExternalAchi~",
                        column: x => x.ExternalAchievementId,
                        principalTable: "ExternalAchievements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAchievementMedias_ExternalAchievementId",
                table: "ExternalAchievementMedias",
                column: "ExternalAchievementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalAchievementMedias");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "ExternalAchievements",
                type: "text",
                nullable: true);
        }
    }
}
