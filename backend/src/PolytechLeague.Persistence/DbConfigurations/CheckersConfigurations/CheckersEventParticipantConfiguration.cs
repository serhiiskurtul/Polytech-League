using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Persistence.DbConfigurations.CheckersConfigurations;

public class CheckersEventParticipantConfiguration : IEntityTypeConfiguration<CheckersEventParticipant>
{   
    public void Configure(EntityTypeBuilder<CheckersEventParticipant> builder)
    {
        builder.ToTable("checkers_event_participants");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.TournamentId).HasDatabaseName("IX_checkers_event_participants_event_id");
        builder.HasIndex(x => x.PlayerId).HasDatabaseName("IX_checkers_event_participants_user_id");
        builder.HasIndex(x => new { x.TournamentId, x.PlayerId }).HasDatabaseName("IX_checkers_event_participants_event_user").IsUnique();
        builder.HasIndex(x => new { x.TournamentId, x.Points }).HasDatabaseName("IX_checkers_event_participants_event_points");

        builder.Property(x => x.Points)
            .HasColumnName("points")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(x => x.StartingRating)
            .HasColumnName("starting_rating")
            .IsRequired();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
