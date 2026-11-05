
namespace ServiceDesk.Identity.Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string CorporateEmail { get; private set; }
    public string DisplayName { get; private set; }
    public bool IsActive { get; private set; }

    public User(string corporateEmail, string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(corporateEmail);
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        Id = Guid.NewGuid();
        CorporateEmail = corporateEmail.Trim().ToLowerInvariant();
        DisplayName = displayName.Trim();
        IsActive = true;
    }

    public void UpdateProfile(string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
        DisplayName = displayName.Trim();
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
