using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball3x3;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations.Basketball3x3Configurations;

public class Basketball3x3MatchConfiguration : IEntityTypeConfiguration<Basketball3x3Match>
{
    public void Configure(EntityTypeBuilder<Basketball3x3Match> builder)
    {
        builder.ToTable("basketball3x3_matches");
        
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
                "basketball3x3_match_home_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<Basketball3x3Match>().WithMany().HasForeignKey("Basketball3x3MatchId")
            );

        builder.HasMany(m => m.AwayRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "basketball3x3_match_away_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<Basketball3x3Match>().WithMany().HasForeignKey("Basketball3x3MatchId")
            );
    }
}
