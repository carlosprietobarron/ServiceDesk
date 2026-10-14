using ServiceDesk.Tickets.Domain.Users;

namespace ServiceDesk.Tickets.Domain.Tickets;

public sealed class Ticket
{
    private readonly List<TicketHistoryEntry> _history = new();

    public Guid Id { get; private set; }
    public TicketNumber Number { get; private set; } = null!;
    public string Title { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public UserId RequesterId { get; private set; } = null!;
    public TicketState State { get; private set; }
    public bool IsSensitive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public IReadOnlyCollection<TicketHistoryEntry> History =>
        _history.AsReadOnly();

    private Ticket()
    {
        // Constructor para EF Core.
    }

    public Ticket(
        TicketNumber number,
        string title,
        string description,
        UserId requesterId,
        UserId createdBy,
        bool isSensitive,
        DateTime createdAt)
    {
        ArgumentNullException.ThrowIfNull(number);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentNullException.ThrowIfNull(requesterId);
        ArgumentNullException.ThrowIfNull(createdBy);

        if (createdAt.Kind != DateTimeKind.Utc)
            throw new ArgumentException(
                "La fecha debe estar expresada en UTC.",
                nameof(createdAt));

        Id = Guid.NewGuid();
        Number = number;
        Title = title.Trim();
        Description = description.Trim();
        RequesterId = requesterId;
        State = TicketState.New;
        IsSensitive = isSensitive;
        CreatedAt = createdAt;

        _history.Add(new TicketHistoryEntry(
            Id,
            "TicketCreated",
            null,
            State.ToString(),
            createdBy,
            CreatedAt));
    }
}