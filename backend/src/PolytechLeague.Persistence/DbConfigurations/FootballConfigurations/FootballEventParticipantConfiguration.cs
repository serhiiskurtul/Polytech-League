using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballEventParticipantConfiguration : IEntityTypeConfiguration<FootballEventParticipant>
{
    public void Configure(EntityTypeBuilder<FootballEventParticipant> builder)
    {
        builder.ToTable("Football_event_participants");

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
