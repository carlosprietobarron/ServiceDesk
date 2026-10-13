using Microsoft.EntityFrameworkCore;
using ServiceDesk.Tickets.Domain.Users;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(TicketsDbContext).Assembly);
    }
}