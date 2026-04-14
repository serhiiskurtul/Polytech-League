using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonMatchConfiguration : IEntityTypeConfiguration<BadmintonMatch>
{
    public void Configure(EntityTypeBuilder<BadmintonMatch> builder)
    {
        builder.ToTable("badminton_matches");

        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Event)
            .WithMany(e => e.Matches)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.Player1)
            .WithMany()
            .HasForeignKey(m => m.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Player2)
            .WithMany()
            .HasForeignKey(m => m.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.WinnerPlayer)
            .WithMany()
            .HasForeignKey(m => m.WinnerPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.HomeTeam)
            .WithMany()
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayTeam)
            .WithMany()
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.WinnerTeam)
            .WithMany()
            .HasForeignKey(m => m.WinnerTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Sets)
            .WithOne(s => s.Match)
            .HasForeignKey(s => s.MatchId);

        builder.HasMany(m => m.HomeRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "badminton_match_home_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<BadmintonMatch>().WithMany().HasForeignKey("BadmintonMatchId")
            );

        builder.HasMany(m => m.AwayRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "badminton_match_away_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<BadmintonMatch>().WithMany().HasForeignKey("BadmintonMatchId")
            );
    }
}
