using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Persistence.DbConfigurations.Basketball3x3Configurations;

public class Basketball3x3EventParticipantConfiguration : IEntityTypeConfiguration<Basketball3x3EventParticipant>
{
    public void Configure(EntityTypeBuilder<Basketball3x3EventParticipant> builder)
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
