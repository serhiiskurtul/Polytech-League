using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration;

public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("players");

        builder.Property(x => x.City)
            .HasColumnName("city")
            .IsRequired();

        builder.Property(x => x.Group)
            .HasColumnName("group")
            .IsRequired();
    }
}
