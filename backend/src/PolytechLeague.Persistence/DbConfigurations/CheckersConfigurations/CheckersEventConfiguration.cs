using PolytechLeague.Domain.Entities.Checkers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration.CheckersConfigurations;

public class CheckersEventConfiguration : IEntityTypeConfiguration<CheckersEvent>
{
    public void Configure(EntityTypeBuilder<CheckersEvent> builder)
    {
        builder.ToTable("checkers_events");

        builder.HasIndex(x => x.System).HasDatabaseName("IX_checkers_events_system");
        builder.HasIndex(x => x.Variant).HasDatabaseName("IX_checkers_events_variant");

        builder.Property(x => x.TimeControl)
            .HasColumnName("time_control")
            .IsRequired();

        builder.Property(x => x.ParticipantsCount)
            .HasColumnName("participants_count")
            .IsRequired();

        builder.Property(x => x.System)
            .HasColumnName("system")
            .IsRequired();
            
        builder.Property(x => x.Variant)
            .HasColumnName("variant")
            .IsRequired();

        builder.HasMany(x => x.Games)
            .WithOne()
            .HasForeignKey(x => x.CheckersEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Players)
            .WithOne()
            .HasForeignKey(x => x.CheckersEventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
