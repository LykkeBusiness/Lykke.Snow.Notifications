using System;

namespace Lykke.Snow.Notifications.SqlRepositories.Exceptions
{
    public sealed class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(object entity) : base("The entity already exists in the database.")
        {
            Data.Add("entity", entity);
        }
    }
}
