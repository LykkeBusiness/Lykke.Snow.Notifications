using System;
using System.Linq;
using FsCheck;
using Lykke.Snow.FirebaseIntegration;
using Lykke.Snow.Notifications.Domain.Enums;
using Lykke.Snow.Notifications.Domain.Model;

namespace Lykke.Snow.Notifications.Tests
{
    internal static class Gens
    {
        internal static Gen<DeviceConfiguration.Notification> Notification =>
            from type in Gen.Elements(Enum.GetNames(typeof(NotificationType))
                .Where(t => Domain.Model.DeviceConfiguration.Notification.ParseType(t) != null))
            from enabled in Arb.Default.Bool().Generator
            select new DeviceConfiguration.Notification(type, enabled);

        internal static Gen<DeviceConfiguration.Notification[]> Notifications =>
            Gen.NonEmptyListOf(Notification)
                .Select(ns => ns.DistinctBy(x => x.Type).ToArray());

        internal static Gen<Locale> Locale =>
            Gen.Elements(Enum.GetNames(typeof(Locale))).Select(l => (Locale)Enum.Parse(typeof(Locale), l));

        internal static Gen<DeviceConfiguration> DeviceConfiguration =>
            from deviceId in Arb.Default.NonWhiteSpaceString().Generator
            from accountId in Arb.Default.NonWhiteSpaceString().Generator
            from locale in Locale
            from notifications in Notifications
            select new DeviceConfiguration(deviceId.Get, accountId.Get, locale.ToString(), notifications);

        internal static Gen<DateTime> RegisteredOn =>
            from dt in Arb.Default.DateTime().Generator
            where dt < DateTime.UtcNow
            select dt;
                

        internal static Gen<DeviceRegistration> DeviceRegistration =>
            from deviceId in Arb.Default.NonWhiteSpaceString().Generator
            from accountId in Arb.Default.NonWhiteSpaceString().Generator
            from deviceToken in Arb.Default.NonWhiteSpaceString().Generator
            from registeredOn in RegisteredOn
            select new DeviceRegistration(accountId.Get, deviceToken.Get, deviceId.Get, registeredOn);

        internal static Gen<ProxyConfiguration> ProxyConfigurationWithCredentials  =>
            from address in Arb.Default.NonWhiteSpaceString().Generator
            from username in Arb.Default.NonWhiteSpaceString().Generator
            from password in Arb.Default.NonWhiteSpaceString().Generator
            select new ProxyConfiguration(address.Get, username.Get, password.Get);
    }
}
