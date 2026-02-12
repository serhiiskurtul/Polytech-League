using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration.ChessConfigurations;

public class ChessGameConfiguration : IEntityTypeConfiguration<ChessGame>
{
    public void Configure(EntityTypeBuilder<ChessGame> builder)
    {
        builder.ToTable("chess_games");

        // ---------- Indexes ----------
        builder.HasIndex(x => x.TournamentId)
            .HasDatabaseName("IX_chess_games_tournament_id");

        builder.HasIndex(x => new { x.TournamentId, x.Round })
            .HasDatabaseName("IX_chess_games_tournament_round");

        builder.HasIndex(x => new { x.TournamentId, x.WhitePlayerId })
            .HasDatabaseName("IX_chess_games_tournament_white");

        builder.HasIndex(x => new { x.TournamentId, x.BlackPlayerId })
            .HasDatabaseName("IX_chess_games_tournament_black");

        builder.HasIndex(x => x.Result)
            .HasDatabaseName("IX_chess_games_result");

        // ---------- Columns ----------
        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
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
            .HasColumnName("round")
            .IsRequired();

        builder.Property(x => x.Board)
            .HasColumnName("board")
            .IsRequired();

        // ---------- Relationships ----------

        // Tournament → Games
        builder.HasOne(g => g.Tournament)
            .WithMany()
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
