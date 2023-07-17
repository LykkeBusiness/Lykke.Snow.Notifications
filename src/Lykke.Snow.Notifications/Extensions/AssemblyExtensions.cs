using System;
using System.Linq;
using System.Reflection;

namespace Lykke.Snow.Notifications.Extensions
{
    internal static class AssemblyExtensions
    {
        public static string Attribute<T>(this ICustomAttributeProvider provider, Func<T, string> property)
        {
            var value = provider.GetCustomAttributes(typeof(T), false).Cast<T>().FirstOrDefault();
            return value == null ? string.Empty : property(value);
        }
    }
}