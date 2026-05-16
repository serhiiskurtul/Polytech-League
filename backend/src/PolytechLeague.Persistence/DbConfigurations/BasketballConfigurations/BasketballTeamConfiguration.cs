using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball;

namespace PolytechLeague.Persistence.DbConfigurations.BasketballConfigurations;

public class BasketballTeamConfiguration : IEntityTypeConfiguration<BasketballTeam>
{
    public void Configure(EntityTypeBuilder<BasketballTeam> builder)
    {
        builder.ToTable("basketball3x3_teams");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Rank).HasColumnName("rank");
    }
}
