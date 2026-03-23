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

        builder.HasQueryFilter(p => !p.User.IsDeleted);

        builder.HasIndex(x => x.CheckersEventId).HasDatabaseName("IX_checkers_event_participants_event_id");
        builder.HasIndex(x => x.UserId).HasDatabaseName("IX_checkers_event_participants_user_id");
        builder.HasIndex(x => new { x.CheckersEventId, x.UserId }).HasDatabaseName("IX_checkers_event_participants_event_user").IsUnique();
        builder.HasIndex(x => new { x.CheckersEventId, x.PointsEarned }).HasDatabaseName("IX_checkers_event_participants_event_points");

        builder.Property(x => x.PointsEarned)
            .HasColumnName("points_earned")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(x => x.StartingRating)
            .HasColumnName("starting_rating")
            .IsRequired();

        builder.HasOne(x => x.Event)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.CheckersEventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
