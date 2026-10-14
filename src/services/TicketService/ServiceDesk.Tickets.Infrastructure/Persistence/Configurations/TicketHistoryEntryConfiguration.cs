using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ServiceDesk.Tickets.Domain.Tickets;
using ServiceDesk.Tickets.Domain.Users;

namespace ServiceDesk.Tickets.Infrastructure.Persistence.Configurations;

public sealed class TicketHistoryEntryConfiguration
    : IEntityTypeConfiguration<TicketHistoryEntry>
{
    public void Configure(
        EntityTypeBuilder<TicketHistoryEntry> builder)
    {
        builder.ToTable("ticket_history");

        builder.HasKey(entry => entry.Id);

        builder.Property(entry => entry.Id)
            .ValueGeneratedNever();

        builder.Property(entry => entry.EventType)
            .IsRequired();

        builder.Property(entry => entry.FromValue)
            .IsRequired(false);

        builder.Property(entry => entry.ToValue)
            .IsRequired(false);

        builder.Property(entry => entry.ActorId)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .IsRequired();

        builder.Property(entry => entry.OccurredAt)
            .IsRequired();

        builder.HasOne<UserReference>()
            .WithMany()
            .HasForeignKey(entry => entry.ActorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(entry => new
        {
            entry.TicketId,
            entry.OccurredAt
        });
    }
}