using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonPlayerConfiguration : IEntityTypeConfiguration<BadmintonPlayer>
{
    public void Configure(EntityTypeBuilder<BadmintonPlayer> builder)
    {
        builder.ToTable("badminton_players");
    }
}
