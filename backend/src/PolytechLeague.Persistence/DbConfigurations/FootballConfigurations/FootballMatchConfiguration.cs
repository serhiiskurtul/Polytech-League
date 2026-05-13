using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballMatchConfiguration : IEntityTypeConfiguration<FootballMatch>
{
    public void Configure(EntityTypeBuilder<FootballMatch> builder)
    {
        builder.ToTable("Football_matches");
        
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

        builder.HasMany(m => m.HomeRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Football_match_home_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<FootballMatch>().WithMany().HasForeignKey("FootballMatchId")
            );

        builder.HasMany(m => m.AwayRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "Football_match_away_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<FootballMatch>().WithMany().HasForeignKey("FootballMatchId")
            );

        builder.HasOne(m => m.HomeGoalkeeper)
            .WithMany()
            .HasForeignKey(m => m.HomeGoalkeeperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayGoalkeeper)
            .WithMany()
            .HasForeignKey(m => m.AwayGoalkeeperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.HomeCaptain)
            .WithMany()
            .HasForeignKey(m => m.HomeCaptainId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.AwayCaptain)
            .WithMany()
            .HasForeignKey(m => m.AwayCaptainId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
