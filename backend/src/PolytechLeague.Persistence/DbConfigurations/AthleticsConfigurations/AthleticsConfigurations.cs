using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Athletics;

namespace PolytechLeague.Persistence.DbConfigurations.AthleticsConfigurations;

public class AthleticsEventConfiguration : IEntityTypeConfiguration<AthleticsEvent>
{
    public void Configure(EntityTypeBuilder<AthleticsEvent> builder)
    {
        builder.HasMany(e => e.Results)
            .WithOne(r => r.AthleticsEvent)
            .HasForeignKey(r => r.AthleticsEventId);
            
        builder.HasMany(e => e.Participants)
            .WithOne(p => p.AthleticsEvent)
            .HasForeignKey(p => p.AthleticsEventId);
    }
}

public class AthleticsEventParticipantConfiguration : IEntityTypeConfiguration<AthleticsEventParticipant>
{
    public void Configure(EntityTypeBuilder<AthleticsEventParticipant> builder)
    {
        builder.HasKey(p => new { p.AthleticsEventId, p.PlayerId });
    }
}

public class AthleticsResultConfiguration : IEntityTypeConfiguration<AthleticsResult>
{
    public void Configure(EntityTypeBuilder<AthleticsResult> builder)
    {
        builder.HasKey(r => r.Id);
    }
}
