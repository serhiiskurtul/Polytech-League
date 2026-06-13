using PolytechLeague.Domain.Entities.Chess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

public class ChessEventConfiguration : IEntityTypeConfiguration<ChessEvent>
{
    public void Configure(EntityTypeBuilder<ChessEvent> builder)
    {
        builder.ToTable("chess_events");

        builder.HasIndex(x => x.System).HasDatabaseName("IX_chess_events_system");

        builder.Property(x => x.TimeControl)
            .HasColumnName("time_control")
            .IsRequired(false);

        builder.Property(x => x.ParticipantsCount)
            .HasColumnName("participants_count")
            .IsRequired();

        builder.Property(x => x.System)
            .HasColumnName("system")
            .IsRequired(false);
    }
}
