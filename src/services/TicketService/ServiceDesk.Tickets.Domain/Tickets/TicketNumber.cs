namespace ServiceDesk.Tickets.Domain.Tickets;

public sealed record TicketNumber
{
    public string Value { get; }

    public TicketNumber(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        Value = value.Trim();
    }

    public override string ToString() => Value;
}