using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Armwrestling;

namespace PolytechLeague.Persistence.DbConfigurations.ArmwrestlingConfigurations;

public class ArmwrestlingEventConfiguration : IEntityTypeConfiguration<ArmwrestlingEvent>
{
    public void Configure(EntityTypeBuilder<ArmwrestlingEvent> builder)
    {
        builder.ToTable("armwrestling_events");

        builder.HasIndex(x => x.Hand).HasDatabaseName("IX_arm_events_hand");
        builder.HasIndex(x => x.WeightClass).HasDatabaseName("IX_arm_events_weight_class");

        builder.Property(x => x.Hand)
            .HasColumnName("hand")
            .IsRequired();

        builder.Property(x => x.WeightClass)
            .HasColumnName("weight_class")
            .IsRequired();
    }
}
