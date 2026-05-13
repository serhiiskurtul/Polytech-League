using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballEventConfiguration : IEntityTypeConfiguration<FootballEvent>
{
    public void Configure(EntityTypeBuilder<FootballEvent> builder)
    {
        builder.ToTable("Football_events");

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
