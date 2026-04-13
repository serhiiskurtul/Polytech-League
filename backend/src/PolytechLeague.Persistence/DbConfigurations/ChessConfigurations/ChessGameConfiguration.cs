using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

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
            .IsRequired(false);

        builder.Property(x => x.BlackPlayerId)
            .HasColumnName("black_player_id")
            .IsRequired(false);

        builder.Property(x => x.WhiteTeamId)
            .HasColumnName("white_team_id")
            .IsRequired(false);

        builder.Property(x => x.BlackTeamId)
            .HasColumnName("black_team_id")
            .IsRequired(false);

        builder.Property(x => x.WhiteTeamScore)
            .HasColumnName("white_team_score")
            .IsRequired(false);

        builder.Property(x => x.BlackTeamScore)
            .HasColumnName("black_team_score")
            .IsRequired(false);

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
        builder.HasOne(x => x.Tournament)
            .WithMany(x => x.Games)
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.WhitePlayer)
            .WithMany()
            .HasForeignKey(x => x.WhitePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BlackPlayer)
            .WithMany()
            .HasForeignKey(x => x.BlackPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.WhiteTeam)
            .WithMany()
            .HasForeignKey(x => x.WhiteTeamId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.BlackTeam)
            .WithMany()
            .HasForeignKey(x => x.BlackTeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
