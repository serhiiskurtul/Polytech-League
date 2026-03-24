using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonSetConfiguration : IEntityTypeConfiguration<BadmintonSet>
{
    public void Configure(EntityTypeBuilder<BadmintonSet> builder)
    {
        builder.ToTable("badminton_sets");
    }
}
