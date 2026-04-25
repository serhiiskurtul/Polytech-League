using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Basketball3x3;

namespace PolytechLeague.Persistence.DbConfigurations.Basketball3x3Configurations;

public class Basketball3x3TeamConfiguration : IEntityTypeConfiguration<Basketball3x3Team>
{
    public void Configure(EntityTypeBuilder<Basketball3x3Team> builder)
    {
        builder.ToTable("basketball3x3_teams");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Rank).HasColumnName("rank");
    }
}
