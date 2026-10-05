using Microsoft.EntityFrameworkCore;

namespace ServiceDesk.Tickets.Infrastructure.Persistence;

public class TicketsDbContext : DbContext
{
    public TicketsDbContext(DbContextOptions<TicketsDbContext> options)
        : base(options)
    {
    }
}