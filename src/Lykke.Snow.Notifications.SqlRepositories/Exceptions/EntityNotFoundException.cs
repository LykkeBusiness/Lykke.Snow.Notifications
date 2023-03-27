using System;

namespace Lykke.Snow.Notifications.SqlRepositories.Exceptions
{
    public sealed class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }

        public EntityNotFoundException(int key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }
    }
}
