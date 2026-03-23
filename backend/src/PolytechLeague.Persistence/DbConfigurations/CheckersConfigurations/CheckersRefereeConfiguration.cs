using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities.Checkers;

namespace PolytechLeague.Persistence.DbConfigurations.CheckersConfigurations;

public class CheckersRefereeConfiguration : IEntityTypeConfiguration<CheckersReferee>
{
    public void Configure(EntityTypeBuilder<CheckersReferee> builder)
    {
        builder.ToTable("checkers_referees");
    }
}
