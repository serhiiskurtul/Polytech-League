using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;

public class FutsalEventParticipantConfiguration : IEntityTypeConfiguration<FutsalEventParticipant>
{
    public void Configure(EntityTypeBuilder<FutsalEventParticipant> builder)
    {
        builder.ToTable("futsal_event_participants");

        builder.HasKey(x => new { x.EventId, x.PlayerId });

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Participants)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
