using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Badminton;

namespace PolytechLeague.Persistence.DbConfigurations.BadmintonConfigurations;

public class BadmintonTeamConfiguration : IEntityTypeConfiguration<BadmintonTeam>
{
    public void Configure(EntityTypeBuilder<BadmintonTeam> builder)
    {
        builder.ToTable("badminton_teams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(100);

        builder.HasOne(t => t.Player1)
            .WithMany()
            .HasForeignKey(t => t.Player1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Player2)
            .WithMany()
            .HasForeignKey(t => t.Player2Id)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(t => t.Event)
            .WithMany(e => e.Teams)
            .HasForeignKey(t => t.EventId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
