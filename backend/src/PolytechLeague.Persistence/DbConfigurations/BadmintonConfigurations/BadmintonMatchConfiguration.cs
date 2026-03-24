using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonMatchConfiguration : IEntityTypeConfiguration<BadmintonMatch>
{
    public void Configure(EntityTypeBuilder<BadmintonMatch> builder)
    {
        builder.ToTable("badminton_matches");

        builder.HasOne(m => m.Player1)
            .WithMany()
            .HasForeignKey(m => m.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Player2)
            .WithMany()
            .HasForeignKey(m => m.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Winner)
            .WithMany()
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(m => m.Sets)
            .WithOne(s => s.Match)
            .HasForeignKey(s => s.MatchId);
    }
}
