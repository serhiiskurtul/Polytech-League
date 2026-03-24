using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballEventConfiguration : IEntityTypeConfiguration<VolleyballEvent>
{
    public void Configure(EntityTypeBuilder<VolleyballEvent> builder)
    {
        builder.ToTable("volleyball_events");

        builder.HasMany(e => e.Matches)
            .WithOne(m => m.Event)
            .HasForeignKey(m => m.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
