using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Volleyball;

namespace PolytechLeague.Persistence.DbConfigurations.VolleyballConfigurations;

public class VolleyballTeamConfiguration : IEntityTypeConfiguration<VolleyballTeam>
{
    public void Configure(EntityTypeBuilder<VolleyballTeam> builder)
    {
        builder.ToTable("volleyball_teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
    }
}
