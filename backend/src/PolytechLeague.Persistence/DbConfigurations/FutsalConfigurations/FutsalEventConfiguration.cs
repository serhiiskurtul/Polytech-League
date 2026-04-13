using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;

public class FutsalEventConfiguration : IEntityTypeConfiguration<FutsalEvent>
{
    public void Configure(EntityTypeBuilder<FutsalEvent> builder)
    {
        builder.ToTable("futsal_events");

        builder.HasMany(e => e.Matches)
            .WithOne(m => m.Event)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Teams)
            .WithOne(t => t.Event)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.FirstPlaceTeam)
            .WithMany()
            .HasForeignKey(e => e.FirstPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.SecondPlaceTeam)
            .WithMany()
            .HasForeignKey(e => e.SecondPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.ThirdPlaceTeam)
            .WithMany()
            .HasForeignKey(e => e.ThirdPlaceTeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
