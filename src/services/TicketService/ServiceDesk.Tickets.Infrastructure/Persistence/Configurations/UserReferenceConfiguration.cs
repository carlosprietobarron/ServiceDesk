using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceDesk.Tickets.Domain.Users;

namespace ServiceDesk.Tickets.Infrastructure.Persistence.Configurations;

public sealed class UserReferenceConfiguration
    : IEntityTypeConfiguration<UserReference>
{
    public void Configure(EntityTypeBuilder<UserReference> builder)
    {
        builder.ToTable("user_references");

        builder.HasKey(user => user.UserId);

        builder.Property(user => user.UserId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        builder.Property(user => user.DisplayName)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();
    }
}