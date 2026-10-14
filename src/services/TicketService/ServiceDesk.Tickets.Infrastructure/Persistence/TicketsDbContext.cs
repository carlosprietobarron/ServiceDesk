using Microsoft.EntityFrameworkCore;
using ServiceDesk.Tickets.Domain.Users;
using ServiceDesk.Tickets.Domain.Tickets;

namespace ServiceDesk.Tickets.Infrastructure.Persistence;

public class TicketsDbContext : DbContext
{
    public TicketsDbContext(
        DbContextOptions<TicketsDbContext> options)
        : base(options)
    {
    }


    public DbSet<UserReference> UserReferences =>
        Set<UserReference>();

    public DbSet<Ticket> Tickets =>
    Set<Ticket>();

    public DbSet<TicketHistoryEntry> TicketHistoryEntries =>
        Set<TicketHistoryEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TicketsDbContext).Assembly);
    }
}