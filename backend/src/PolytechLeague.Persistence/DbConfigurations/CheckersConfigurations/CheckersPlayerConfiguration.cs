using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Persistence.DbConfigurations.CheckersConfigurations;

public class CheckersPlayerConfiguration : IEntityTypeConfiguration<CheckersPlayer>
{
    public void Configure(EntityTypeBuilder<CheckersPlayer> builder)
    {
        builder.ToTable("checkers_players");

        builder.HasIndex(x => x.Rating)
            .HasDatabaseName("IX_checkers_players_rating");

        builder.Property(x => x.Rating).HasColumnName("rating");

        builder.Property(x => x.Title)
            .HasColumnName("title")
            .IsRequired();
    }
}
