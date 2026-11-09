using ServiceDesk.Identity.Application.Abstractions;
using ServiceDesk.Identity.Domain.Users;

namespace ServiceDesk.Identity.Application.Features.Users.CreateUser;

public sealed class CreateUserHandler
{
    //dependency injection for the user repository
    private readonly IUserRepository _userRepository;
    //constructor for the CreateUserHandler class
    public CreateUserHandler(IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(userRepository);
        _userRepository = userRepository;
    }

    public async Task<User> HandleAsync(
        string corporateEmail,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(corporateEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        var normalizedEmail = corporateEmail.Trim().ToLowerInvariant();

        var existingUser = await _userRepository.GetByEmailAsync(
            normalizedEmail,
            cancellationToken);

        if (existingUser is not null)
        {
            throw new InvalidOperationException(
                "A user with this corporate email already exists.");
        }

        var user = new User(normalizedEmail, displayName);

        await _userRepository.AddAsync(user, cancellationToken);

        return user;
    }

}