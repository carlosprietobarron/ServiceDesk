using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceDesk.Tickets.Domain.Tickets;
using ServiceDesk.Tickets.Domain.Users;

namespace ServiceDesk.Tickets.Infrastructure.Persistence.Configurations;

public sealed class TicketConfiguration
    : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");

        builder.HasKey(ticket => ticket.Id);

        builder.Property(ticket => ticket.Id)
            .ValueGeneratedNever();

        builder.Property(ticket => ticket.Number)
            .HasConversion(
                number => number.Value,
                value => new TicketNumber(value))
            .IsRequired();

        builder.HasIndex(ticket => ticket.Number)
            .IsUnique();

        builder.Property(ticket => ticket.Title)
            .IsRequired();

        builder.Property(ticket => ticket.Description)
            .IsRequired();

        builder.Property(ticket => ticket.RequesterId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(ticket => ticket.State)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(ticket => ticket.CreatedAt)
            .IsRequired();

        builder.Property(ticket => ticket.IsSensitive)
            .IsRequired();

        builder.HasOne<UserReference>()
            .WithMany()
            .HasForeignKey(ticket => ticket.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(ticket => ticket.History)
            .WithOne()
            .HasForeignKey(entry => entry.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Navigation(ticket => ticket.History)
            .HasField("_history")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}