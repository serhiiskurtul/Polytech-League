using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonEventConfiguration : IEntityTypeConfiguration<BadmintonEvent>
{
    public void Configure(EntityTypeBuilder<BadmintonEvent> builder)
    {
        builder.ToTable("badminton_events");
        
        builder.HasMany(e => e.Matches)
            .WithOne(m => m.Tournament)
            .HasForeignKey(m => m.TournamentId);

        builder.HasMany(e => e.Participants)
            .WithOne(p => p.Tournament)
            .HasForeignKey(p => p.TournamentId);

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
