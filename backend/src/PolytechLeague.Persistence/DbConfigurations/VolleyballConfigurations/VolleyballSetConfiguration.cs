using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballSetConfiguration : IEntityTypeConfiguration<VolleyballSet>
{
    public void Configure(EntityTypeBuilder<VolleyballSet> builder)
    {
        builder.ToTable("volleyball_sets");

        builder.Property(s => s.HomeScore).HasDefaultValue(0);
        builder.Property(s => s.AwayScore).HasDefaultValue(0);

        builder.HasIndex(s => new { s.MatchId, s.SetNumber })
            .IsUnique()
            .HasDatabaseName("IX_volleyball_sets_match_setnumber");
    }
}
