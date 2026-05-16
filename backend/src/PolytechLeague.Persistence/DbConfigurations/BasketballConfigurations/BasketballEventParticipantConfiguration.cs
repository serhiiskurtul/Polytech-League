using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball;

namespace PolytechLeague.Persistence.DbConfigurations.BasketballConfigurations;

public class BasketballEventParticipantConfiguration : IEntityTypeConfiguration<BasketballEventParticipant>
{
    public void Configure(EntityTypeBuilder<BasketballEventParticipant> builder)
    {
        builder.ToTable("basketball3x3_event_participants");

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
