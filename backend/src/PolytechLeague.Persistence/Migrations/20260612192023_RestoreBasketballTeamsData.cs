using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PolytechLeague.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RestoreBasketballTeamsData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO basketball_team_players (""BasketballTeamsId"", ""PlayersId"")
                SELECT DISTINCT m.""HomeTeamId"", r.""PlayerId""
                FROM basketball3x3_match_home_roster r
                JOIN basketball3x3_matches m ON r.""BasketballMatchId"" = m.""Id""
                WHERE m.""HomeTeamId"" IS NOT NULL
                ON CONFLICT DO NOTHING;
            ");

            migrationBuilder.Sql(@"
                INSERT INTO basketball_team_players (""BasketballTeamsId"", ""PlayersId"")
                SELECT DISTINCT m.""AwayTeamId"", r.""PlayerId""
                FROM basketball3x3_match_away_roster r
                JOIN basketball3x3_matches m ON r.""BasketballMatchId"" = m.""Id""
                WHERE m.""AwayTeamId"" IS NOT NULL
                ON CONFLICT DO NOTHING;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
