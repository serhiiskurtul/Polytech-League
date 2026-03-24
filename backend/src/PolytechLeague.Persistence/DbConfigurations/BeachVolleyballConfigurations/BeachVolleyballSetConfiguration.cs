using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Persistence.DbConfigurations.BeachVolleyballConfigurations;

public class BeachVolleyballSetConfiguration : IEntityTypeConfiguration<BeachVolleyballSet>
{
    public void Configure(EntityTypeBuilder<BeachVolleyballSet> builder)
    {
        builder.ToTable("beach_volleyball_sets");

        builder.Property(s => s.HomeScore).HasDefaultValue(0);
        builder.Property(s => s.AwayScore).HasDefaultValue(0);

        builder.HasIndex(s => new { s.MatchId, s.SetNumber })
            .IsUnique()
            .HasDatabaseName("IX_beach_volleyball_sets_match_setnumber");
    }
}
