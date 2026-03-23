using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Persistence.DbConfigurations.ArmwrestlingConfigurations;

public class ArmwrestlingPlayerConfiguration : IEntityTypeConfiguration<ArmwrestlingPlayer>
{
    public void Configure(EntityTypeBuilder<ArmwrestlingPlayer> builder)
    {
        builder.ToTable("armwrestling_players");

        builder.HasIndex(x => x.PreferredWeightClass).HasDatabaseName("IX_arm_players_weight_class");

        builder.Property(x => x.CurrentWeight)
            .HasColumnName("current_weight");

        builder.Property(x => x.PreferredWeightClass)
            .HasColumnName("preferred_weight_class")
            .IsRequired();
    }
}
