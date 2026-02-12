using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PolytechLeague.Infrastructure.Persistence.DbConfiguration;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.ToTable(t => t.HasCheckConstraint("CK_User_CreatedAt", "created_at <= now()"));

        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired();

        builder.Property(x => x.Role)
            .HasColumnName("role")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.Property(x => x.Email).HasColumnName("email");
        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasFilter("is_deleted = false");

        builder.Property(x => x.isEmailConfirmed).HasColumnName("is_email_confirmed");

        builder.Property(x => x.Password)
            .HasColumnName("password")
            .IsRequired();

        builder.Property(x => x.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired();
    }
}
