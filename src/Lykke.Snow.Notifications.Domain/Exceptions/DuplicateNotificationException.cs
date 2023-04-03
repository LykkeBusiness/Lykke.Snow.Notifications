using System;
using System.Collections.Generic;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public sealed class DuplicateNotificationException : Exception
    {
        public DuplicateNotificationException(IReadOnlyList<string> duplicateNotificationTypes)
            : base($"Duplicate notification types: {string.Join(", ", duplicateNotificationTypes)}")
        {
            Data.Add("DuplicateNotificationTypes", duplicateNotificationTypes);
        }
    }
}
