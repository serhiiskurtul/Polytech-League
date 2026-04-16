using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Persistence.DbConfigurations.ArmwrestlingConfigurations;

public class ArmwrestlingEventParticipantConfiguration : IEntityTypeConfiguration<ArmwrestlingEventParticipant>
{
    public void Configure(EntityTypeBuilder<ArmwrestlingEventParticipant> builder)
    {
        builder.ToTable("armwrestling_event_participants");

        builder.HasKey(x => new { x.TournamentId, x.PlayerId });

        builder.HasOne(x => x.Tournament)
            .WithMany(t => t.Players)
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
