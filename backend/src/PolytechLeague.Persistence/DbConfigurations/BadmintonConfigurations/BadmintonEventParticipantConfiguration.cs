using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonEventParticipantConfiguration : IEntityTypeConfiguration<BadmintonEventParticipant>
{
    public void Configure(EntityTypeBuilder<BadmintonEventParticipant> builder)
    {
        builder.ToTable("badminton_event_participants");
        builder.HasKey(p => new { p.EventId, p.PlayerId });

        builder.HasOne(p => p.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(p => p.EventId);

        builder.HasOne(p => p.Player)
            .WithMany()
            .HasForeignKey(p => p.PlayerId);
    }
}
