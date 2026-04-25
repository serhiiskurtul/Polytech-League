using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.BeachVolleyball;

namespace PolytechLeague.Persistence.DbConfigurations.BeachVolleyballConfigurations;

public class BeachVolleyballTeamConfiguration : IEntityTypeConfiguration<BeachVolleyballTeam>
{
    public void Configure(EntityTypeBuilder<BeachVolleyballTeam> builder)
    {
        builder.ToTable("beach_volleyball_teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Rank).HasColumnName("rank");
    }
}
