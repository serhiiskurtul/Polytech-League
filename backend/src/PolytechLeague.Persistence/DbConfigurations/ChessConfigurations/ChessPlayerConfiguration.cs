using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Chess;

namespace PolytechLeague.Persistence.DbConfigurations.ChessConfigurations;

public class ChessPlayerConfiguration : IEntityTypeConfiguration<ChessPlayer>
{
    public void Configure(EntityTypeBuilder<ChessPlayer> builder)
    {
        builder.ToTable("chess_players");

        builder.HasIndex(x => x.FideId)
            .IsUnique()
            .HasDatabaseName("IX_chess_players_fide_id");

        builder.HasIndex(x => x.RatingELO)
            .HasDatabaseName("IX_chess_players_rating_elo");

        builder.Property(x => x.RatingELO).HasColumnName("rating_elo");

        builder.Property(x => x.FideId)
            .HasColumnName("fide_id")
            .IsRequired();

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired();
    }
}
