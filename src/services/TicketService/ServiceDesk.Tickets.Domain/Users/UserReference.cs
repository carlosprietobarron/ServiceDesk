using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceDesk.Tickets.Domain.Users
{

    public sealed class UserReference
    {
        public UserId UserId { get; private set; } = null!;
        public string DisplayName { get; private set; } = null!;
        public bool IsActive { get; private set; }

        private UserReference()
        {
            // EF Core uses constructor.
        }

        public UserReference(
            UserId userId,
            string displayName,
            bool isActive)
        {
            ArgumentNullException.ThrowIfNull(userId);
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            UserId = userId;
            DisplayName = displayName.Trim();
            IsActive = isActive;
        }

        public void UpdateProfile(string displayName, bool isActive)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            DisplayName = displayName.Trim();
            IsActive = isActive;
        }
    }
}
