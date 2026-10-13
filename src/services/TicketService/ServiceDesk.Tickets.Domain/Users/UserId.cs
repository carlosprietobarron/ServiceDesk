using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceDesk.Tickets.Domain.Users
{
    public sealed record UserId
    {
        public string Value { get; }

        public UserId(string corporateEmail)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(corporateEmail);

            Value = corporateEmail.Trim().ToLowerInvariant();
        }

        public override string ToString() => Value;
    }
}
