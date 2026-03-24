using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Persistence.DbConfigurations.BeachVolleyballConfigurations;

public class BeachVolleyballEventConfiguration : IEntityTypeConfiguration<BeachVolleyballEvent>
{
    public void Configure(EntityTypeBuilder<BeachVolleyballEvent> builder)
    {
        builder.ToTable("beach_volleyball_events");

        builder.HasMany(e => e.Matches)
            .WithOne(m => m.Event)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
