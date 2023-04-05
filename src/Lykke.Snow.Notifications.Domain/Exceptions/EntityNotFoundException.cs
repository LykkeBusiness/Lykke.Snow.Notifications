using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
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
        
        public EntityNotFoundException(int[] keys) : base($"One or more requested entities with keys {string.Join(", ", keys)} were not found.")
        {
            Data.Add("key", keys);
        }

        public EntityNotFoundException()
        {
        }
    }
}
