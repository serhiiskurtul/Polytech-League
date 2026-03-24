using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Futsal;

namespace PolytechLeague.Persistence.DbConfigurations.FutsalConfigurations;

public class FutsalTeamConfiguration : IEntityTypeConfiguration<FutsalTeam>
{
    public void Configure(EntityTypeBuilder<FutsalTeam> builder)
    {
        builder.ToTable("futsal_teams");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
    }
}
