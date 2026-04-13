using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballMatchConfiguration : IEntityTypeConfiguration<VolleyballMatch>
{
    public void Configure(EntityTypeBuilder<VolleyballMatch> builder)
    {
        builder.ToTable("volleyball_matches");

        builder.HasKey(m => m.Id);

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
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(m => m.Stage).HasMaxLength(100);

        builder.HasMany(m => m.Sets)
            .WithOne(s => s.Match)
            .HasForeignKey(s => s.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(m => m.HomeRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "volleyball_match_home_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<VolleyballMatch>().WithMany().HasForeignKey("VolleyballMatchId")
            );

        builder.HasMany(m => m.AwayRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "volleyball_match_away_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<VolleyballMatch>().WithMany().HasForeignKey("VolleyballMatchId")
            );
    }

}
