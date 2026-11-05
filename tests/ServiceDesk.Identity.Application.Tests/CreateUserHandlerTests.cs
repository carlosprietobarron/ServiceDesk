
using ServiceDesk.Identity.Application.Abstractions;
using ServiceDesk.Identity.Application.Features.Users.CreateUser;
using ServiceDesk.Identity.Domain.Users;

namespace ServiceDesk.Identity.Application.Tests;

public sealed class CreateUserHandlerTests
{
    [Fact]
    public async Task CreateAsync_ShouldRegisterNewUser()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new CreateUserHandler(repository);

        // Act
        var user = await handler.HandleAsync(
            " ANA@EMPRESA.COM ",
            "Ana García");

        // Assert
        Assert.Equal("ana@empresa.com", user.CorporateEmail);
        Assert.Equal("Ana García", user.DisplayName);
        Assert.True(user.IsActive);
        Assert.Single(repository.Users);
    }

    [Fact]
    public async Task CreateAsync_ShouldRejectDuplicateEmail()
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new CreateUserHandler(repository);

        await handler.HandleAsync(
            "ana@empresa.com",
            "Ana García");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(
                "ANA@EMPRESA.COM",
                "Otra Ana"));

        Assert.Single(repository.Users);
    }

    [Theory]
    [InlineData("", "Ana García")]
    [InlineData("ana@empresa.com", "")]
    public async Task CreateAsync_ShouldRejectInvalidData(
        string email,
        string name)
    {
        // Arrange
        var repository = new FakeUserRepository();
        var handler = new CreateUserHandler(repository);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(email, name));

        Assert.Empty(repository.Users);
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = new();

        public Task<User?> GetByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Users.FirstOrDefault(user => user.Id == id));
        }

        public Task<User?> GetByEmailAsync(
            string corporateEmail,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                Users.FirstOrDefault(user =>
                    user.CorporateEmail == corporateEmail));
        }

        public Task AddAsync(
            User user,
            CancellationToken cancellationToken = default)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }
    }
}
