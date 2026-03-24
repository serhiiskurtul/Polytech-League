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
    }
}
