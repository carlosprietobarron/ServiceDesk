using ServiceDesk.Tickets.Domain.Users;

namespace ServiceDesk.Tickets.Domain.Tickets;

public sealed class TicketHistoryEntry
{
    public Guid Id { get; private set; }
    public Guid TicketId { get; private set; }
    public string EventType { get; private set; } = null!;
    public string? FromValue { get; private set; }
    public string? ToValue { get; private set; }
    public UserId ActorId { get; private set; } = null!;
    public DateTime OccurredAt { get; private set; }

    private TicketHistoryEntry()
    {
        // Constructor para EF Core.
    }

    internal TicketHistoryEntry(
        Guid ticketId,
        string eventType,
        string? fromValue,
        string? toValue,
        UserId actorId,
        DateTime occurredAt)
    {
        if (ticketId == Guid.Empty)
            throw new ArgumentException(
                "El identificador del ticket es obligatorio.",
                nameof(ticketId));

        ArgumentException.ThrowIfNullOrWhiteSpace(eventType);
        ArgumentNullException.ThrowIfNull(actorId);

        if (occurredAt.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "La fecha debe estar expresada en UTC.",
                nameof(occurredAt));

        Id = Guid.NewGuid();
        TicketId = ticketId;
        EventType = eventType.Trim();
        FromValue = fromValue;
        ToValue = toValue;
        ActorId = actorId;
        OccurredAt = occurredAt;
    }
}