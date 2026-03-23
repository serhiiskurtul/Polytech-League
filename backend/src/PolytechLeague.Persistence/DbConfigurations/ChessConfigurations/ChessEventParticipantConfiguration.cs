using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

public class ChessEventParticipantConfiguration : IEntityTypeConfiguration<ChessEventParticipant>
{
    public void Configure(EntityTypeBuilder<ChessEventParticipant> builder)
    {
        builder.ToTable("chess_event_participants");

        builder.HasIndex(x => x.TournamentId).HasDatabaseName("IX_chess_event_participants_tournament_id");
        builder.HasIndex(x => x.PlayerId).HasDatabaseName("IX_chess_event_participants_player_id");
        builder.HasIndex(x => new { x.TournamentId, x.Points, x.Rank }).HasDatabaseName("IX_chess_event_participants_tournament_points_rank");

        builder.HasKey(x => new { x.TournamentId, x.PlayerId });

        builder.Property(x => x.Points)
            .HasColumnName("points")
            .HasColumnType("double precision")
            .IsRequired();

        builder.Property(x => x.Rank)
            .HasColumnName("rank")
            .IsRequired();

        builder.HasOne(x => x.Tournament)
            .WithMany(x => x.Players)
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Player)
            .WithMany()
            .HasForeignKey(x => x.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
