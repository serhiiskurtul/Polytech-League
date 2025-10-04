using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CampusLeague.Domain.Entities.Chess;

namespace CampusLeague.Persistence.DbConfiguration.ChessConfigurations;

public class ChessGameConfiguration : IEntityTypeConfiguration<ChessGame>
{
    public void Configure(EntityTypeBuilder<ChessGame> builder)
    {
        builder.ToTable("chess_games");

        builder.HasIndex(x => x.TournamentId).HasDatabaseName("IX_chess_games_tournament_id");
        builder.HasIndex(x => new { x.TournamentId, x.Round }).HasDatabaseName("IX_chess_games_tournament_round");
        builder.HasIndex(x => new { x.TournamentId, x.WhitePlayerId }).HasDatabaseName("IX_chess_games_tournament_white");
        builder.HasIndex(x => new { x.TournamentId, x.BlackPlayerId }).HasDatabaseName("IX_chess_games_tournament_black");
        builder.HasIndex(x => x.Result).HasDatabaseName("IX_chess_games_result");

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.TournamentId)
            .HasColumnName("tournament_id")
            .IsRequired();

        builder.HasOne(x => x.Tournament)
            .WithMany(e => e.Games)
            .HasForeignKey(x => x.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(x => x.WhitePlayerId)
            .HasColumnName("white_player_id")
            .IsRequired();

        builder.Property(x => x.BlackPlayerId)
            .HasColumnName("black_player_id")
            .IsRequired();

        builder.HasOne<ChessPlayer>()
            .WithMany()
            .HasForeignKey(x => x.WhitePlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ChessPlayer>()
            .WithMany()
            .HasForeignKey(x => x.BlackPlayerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Result)
            .HasColumnName("result")
            .IsRequired();

        builder.Property(x => x.Round)
            .HasColumnName("round")
            .IsRequired();

        builder.Property(x => x.Board)
            .HasColumnName("board")
            .IsRequired();
    }
}
