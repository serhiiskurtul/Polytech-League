using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PolytechLeague.Domain.Entities;

namespace PolytechLeague.Persistence.DbConfigurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("persons");

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Property(x => x.FirstName)
            .HasColumnName("first_name")
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("last_name")
            .IsRequired();

        builder.Property(x => x.PhotoUrl)
            .HasColumnName("photo_url")
            .IsRequired();
    }
}
