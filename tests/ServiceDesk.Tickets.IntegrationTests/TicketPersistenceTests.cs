using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ServiceDesk.Tickets.Domain.Tickets;
using ServiceDesk.Tickets.Domain.Users;
using ServiceDesk.Tickets.Infrastructure.Persistence;

namespace ServiceDesk.Tickets.IntegrationTests;

public sealed class TicketPersistenceTests
{
    [Fact]
    public async Task SavesAndLoadsTicketWithCreationHistory()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets("8bbb66ac-376b-4347-9082-3565f6f8c067")
            .AddEnvironmentVariables()
            .Build();

        var connectionString =
            configuration.GetConnectionString("Tickets");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException(
                "Falta ConnectionStrings:Tickets en los secretos locales.");

        await using var connection =
            new NpgsqlConnection(connectionString);

        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<TicketsDbContext>()
            .UseNpgsql(connection)
            .Options;

        await using var transaction =
            await connection.BeginTransactionAsync();

        var userId = new UserId(
            $"test-{Guid.NewGuid():N}@acme.com");

        var number = new TicketNumber(
            $"TEST-{Guid.NewGuid():N}");

        var createdAt = new DateTime(
            2026, 10, 14, 12, 0, 0, DateTimeKind.Utc);

        Guid ticketId;

        await using (var writeContext = new TicketsDbContext(options))
        {
            await writeContext.Database.UseTransactionAsync(transaction);

            writeContext.UserReferences.Add(
                new UserReference(userId, "Usuario de prueba", true));

            var ticket = new Ticket(
                number,
                "Problema de acceso",
                "El usuario no puede acceder a la aplicación.",
                userId,
                userId,
                false,
                createdAt);

            ticketId = ticket.Id;

            writeContext.Tickets.Add(ticket);

            await writeContext.SaveChangesAsync();
        }

        await using (var readContext = new TicketsDbContext(options))
        {
            await readContext.Database.UseTransactionAsync(transaction);

            var loaded = await readContext.Tickets
                .AsNoTracking()
                .Include(ticket => ticket.History)
                .SingleAsync(ticket => ticket.Id == ticketId);

            Assert.Equal(number, loaded.Number);
            Assert.Equal(userId, loaded.RequesterId);
            Assert.Equal("Problema de acceso", loaded.Title);
            Assert.Equal(TicketState.New, loaded.State);
            Assert.Equal(createdAt, loaded.CreatedAt);

            var history = Assert.Single(loaded.History);

            Assert.Equal(ticketId, history.TicketId);
            Assert.Equal("TicketCreated", history.EventType);
            Assert.Equal(userId, history.ActorId);
            Assert.Null(history.FromValue);
            Assert.Equal("New", history.ToValue);
            Assert.Equal(createdAt, history.OccurredAt);
        }

        await transaction.RollbackAsync();
    }
}