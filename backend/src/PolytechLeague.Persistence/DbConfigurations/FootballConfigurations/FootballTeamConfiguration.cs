using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Football;

namespace PolytechLeague.Persistence.DbConfigurations.FootballConfigurations;

public class FootballTeamConfiguration : IEntityTypeConfiguration<FootballTeam>
{
    public void Configure(EntityTypeBuilder<FootballTeam> builder)
    {
        builder.ToTable("Football_teams");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Points).HasColumnName("points");
        builder.Property(t => t.Rank).HasColumnName("rank");

        builder.HasOne(t => t.Captain)
            .WithMany()
            .HasForeignKey(t => t.CaptainId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Goalkeeper)
            .WithMany()
            .HasForeignKey(t => t.GoalkeeperId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.College)
            .WithMany()
            .HasForeignKey(t => t.CollegeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
