namespace ServiceDesk.Tickets.Domain.Tickets;

public enum TicketState
{
    New = 0,
    Classified = 1,
    Assigned = 2,
    PendingHandoff = 3,
    Resolved = 4,
    Closed = 5
}