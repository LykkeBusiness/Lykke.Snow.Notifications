using System;

namespace Lykke.Snow.Notifications.Domain.Exceptions
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(string key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }

        public EntityNotFoundException(int key) : base($"The requested entity with key {key} was not found.")
        {
            Data.Add("key", key);
        }
        
        public EntityNotFoundException()
        {
        }
    }
}
