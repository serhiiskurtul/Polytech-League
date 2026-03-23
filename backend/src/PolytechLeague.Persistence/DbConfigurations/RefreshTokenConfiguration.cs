using Microsoft.EntityFrameworkCore;
using PolytechLeague.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PolytechLeague.Persistence.DbConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.ToTable(t => t.HasCheckConstraint("CK_RefreshToken_ExpiresAt", "expires_at >= now()"));
        builder.HasIndex(x => new { x.UserId, x.IsRevoked, x.ExpiresAt })
            .HasDatabaseName("IX_refresh_tokens_user_id_active");

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.Token)
            .HasColumnName("token")
            .IsRequired();

        builder.Property(x => x.ExpiresAt)
            .HasColumnName("expires_at")
            .IsRequired()
            .HasConversion(
                v => v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

        builder.Property(x => x.IsRevoked)
            .HasColumnName("is_revoked")
            .IsRequired();

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

    }
}
