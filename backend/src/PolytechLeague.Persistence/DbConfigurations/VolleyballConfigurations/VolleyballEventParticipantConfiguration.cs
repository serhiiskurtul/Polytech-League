using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballEventParticipantConfiguration : IEntityTypeConfiguration<VolleyballEventParticipant>
{
    public void Configure(EntityTypeBuilder<VolleyballEventParticipant> builder)
    {
        builder.ToTable("volleyball_event_participants");

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
