// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq.Expressions;

namespace Lykke.Snow.Notifications.SqlRepositories
{
    internal abstract class Specification<T>
    {
        private Expression<Func<T, bool>>? Criteria { get; set; }
        
        protected void AddCriteria(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            return specification.Criteria ?? (x => false);
        }
    }
}
