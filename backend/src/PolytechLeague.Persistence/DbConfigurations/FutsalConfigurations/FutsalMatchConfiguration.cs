using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Futsal;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;

public class FutsalMatchConfiguration : IEntityTypeConfiguration<FutsalMatch>
{
    public void Configure(EntityTypeBuilder<FutsalMatch> builder)
    {
        builder.ToTable("futsal_matches");
        
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
                "futsal_match_home_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<FutsalMatch>().WithMany().HasForeignKey("FutsalMatchId")
            );

        builder.HasMany(m => m.AwayRoster)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "futsal_match_away_roster",
                j => j.HasOne<Player>().WithMany().HasForeignKey("PlayerId"),
                j => j.HasOne<FutsalMatch>().WithMany().HasForeignKey("FutsalMatchId")
            );
    }
}
