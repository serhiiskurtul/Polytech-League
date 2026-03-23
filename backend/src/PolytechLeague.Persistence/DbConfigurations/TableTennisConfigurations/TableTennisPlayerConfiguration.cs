using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Persistence.DbConfigurations.TableTennisConfigurations;

public class TableTennisPlayerConfiguration : IEntityTypeConfiguration<TableTennisPlayer>
{
    public void Configure(EntityTypeBuilder<TableTennisPlayer> builder)
    {
        builder.ToTable("table_tennis_players");

        builder.HasIndex(x => x.Rating)
            .HasDatabaseName("IX_table_tennis_players_rating");

        builder.Property(x => x.Rating)
            .HasColumnName("rating")
            .HasDefaultValue(0);

        builder.Property(x => x.PlayingHand)
            .HasColumnName("playing_hand")
            .IsRequired();
    }
}
