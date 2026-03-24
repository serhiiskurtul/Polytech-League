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
    }
}
