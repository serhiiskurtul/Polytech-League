using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonEventParticipantConfiguration : IEntityTypeConfiguration<BadmintonEventParticipant>
{
    public void Configure(EntityTypeBuilder<BadmintonEventParticipant> builder)
    {
        builder.ToTable("badminton_event_participants");
        builder.HasKey(p => new { p.TournamentId, p.PlayerId });

        builder.HasOne(p => p.Tournament)
            .WithMany(e => e.Participants)
            .HasForeignKey(p => p.TournamentId);

        builder.HasOne(p => p.Player)
            .WithMany()
            .HasForeignKey(p => p.PlayerId);
    }
}
