using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;

public class TableTennisEventParticipantConfiguration : IEntityTypeConfiguration<TableTennisEventParticipant>
{
    public void Configure(EntityTypeBuilder<TableTennisEventParticipant> builder)
    {
        builder.ToTable("table_tennis_event_participants");

        builder.HasKey(x => new { x.TournamentId, x.PlayerId });

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
            .IsRequired();

        builder.Property(x => x.PlayerId)
            .HasColumnName("player_id")
            .IsRequired();

        builder.Property(x => x.Points)
            .HasColumnName("points")
            .HasDefaultValue(0);

        builder.Property(x => x.Rank)
            .HasColumnName("rank");

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
