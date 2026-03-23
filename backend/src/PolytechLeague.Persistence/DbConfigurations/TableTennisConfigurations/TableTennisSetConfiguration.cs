using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.TableTennis;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration.TableTennisConfigurations;

public class TableTennisSetConfiguration : IEntityTypeConfiguration<TableTennisSet>
{
    public void Configure(EntityTypeBuilder<TableTennisSet> builder)
    {
        builder.ToTable("table_tennis_sets");

        builder.Property(x => x.Id).HasColumnName("id");
        
        builder.Property(x => x.MatchId)
            .HasColumnName("match_id")
            .IsRequired();

        builder.Property(x => x.SetNumber)
            .HasColumnName("set_number")
            .IsRequired();

        builder.Property(x => x.Player1Score)
            .HasColumnName("player1_score")
            .HasDefaultValue(0);

        builder.Property(x => x.Player2Score)
            .HasColumnName("player2_score")
            .HasDefaultValue(0);

        builder.HasIndex(x => new { x.MatchId, x.SetNumber })
            .IsUnique()
            .HasDatabaseName("IX_tt_sets_match_setnumber");
    }
}
