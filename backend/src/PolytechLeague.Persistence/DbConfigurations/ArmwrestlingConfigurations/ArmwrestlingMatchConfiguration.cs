using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Persistence.DbConfigurations.ArmwrestlingConfigurations;

public class ArmwrestlingMatchConfiguration : IEntityTypeConfiguration<ArmwrestlingMatch>
{
    public void Configure(EntityTypeBuilder<ArmwrestlingMatch> builder)
    {
        builder.ToTable("armwrestling_matches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.EventId)
            .HasColumnName("event_id")
            .IsRequired();

        builder.Property(x => x.Player1Id)
            .HasColumnName("player1_id")
            .IsRequired();

        builder.Property(x => x.Player2Id)
            .HasColumnName("player2_id")
            .IsRequired();

        builder.Property(x => x.WinnerId)
            .HasColumnName("winner_id");

        builder.Property(x => x.Stage)
            .HasColumnName("stage")
            .HasMaxLength(50);

        // Relationships
        builder.HasOne(x => x.Event)
            .WithMany(x => x.Matches)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player1)
            .WithMany()
            .HasForeignKey(x => x.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Player2)
            .WithMany()
            .HasForeignKey(x => x.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Winner)
            .WithMany()
            .HasForeignKey(x => x.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasIndex(x => x.EventId).HasDatabaseName("IX_arm_matches_event_id");
        builder.HasIndex(x => x.Player1Id).HasDatabaseName("IX_arm_matches_player1_id");
        builder.HasIndex(x => x.Player2Id).HasDatabaseName("IX_arm_matches_player2_id");
    }
}
