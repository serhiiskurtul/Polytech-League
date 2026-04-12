using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Persistence.DbConfigurations.CheckersConfigurations;

public class CheckersGameConfiguration : IEntityTypeConfiguration<CheckersGame>
{
    public void Configure(EntityTypeBuilder<CheckersGame> builder)
    {
        builder.ToTable("checkers_games");

        // ---------- Indexes ----------
        builder.HasIndex(x => x.TournamentId)
            .HasDatabaseName("IX_checkers_games_event_id");

        builder.HasIndex(x => new { x.TournamentId, x.Round })
            .HasDatabaseName("IX_checkers_games_event_round");

        builder.HasIndex(x => new { x.TournamentId, x.WhitePlayerId })
            .HasDatabaseName("IX_checkers_games_event_white");

        builder.HasIndex(x => new { x.TournamentId, x.BlackPlayerId })
            .HasDatabaseName("IX_checkers_games_event_black");

        builder.HasIndex(x => x.Result)
            .HasDatabaseName("IX_checkers_games_result");

        // ---------- Columns ----------
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TournamentId)
            .HasColumnName("checkers_event_id")
            .IsRequired();

        builder.Property(x => x.WhitePlayerId)
            .HasColumnName("white_player_id")
            .IsRequired();

        builder.Property(x => x.BlackPlayerId)
            .HasColumnName("black_player_id")
            .IsRequired();

        builder.Property(x => x.Result)
            .HasColumnName("result")
            .IsRequired();

        builder.Property(x => x.Round)
            .HasColumnName("round_number")
            .IsRequired();

        builder.Property(x => x.ScheduledAt)
            .HasColumnName("scheduled_at");

        builder.Property(x => x.FinishedAt)
            .HasColumnName("finished_at");

        // ---------- Relationships ----------

        // Tournament → Games
        builder.HasOne(g => g.Event)
            .WithMany(e => e.Games)
            .HasForeignKey(g => g.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        // White player
        builder.HasOne(g => g.WhitePlayer)
            .WithMany()
            .HasForeignKey(g => g.WhitePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Black player
        builder.HasOne(g => g.BlackPlayer)
            .WithMany()
            .HasForeignKey(g => g.BlackPlayerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
