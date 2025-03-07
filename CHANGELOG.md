## 1.13.0 - Nova 2. Delivery 50 (March 07, 2025)
### What's changed
* LT-5905: Proxy configuration usage.


## 1.12.0 - Nova 2. Delivery 48 (December 19, 2024)
### What's changed
* LT-5939: Update refit to 8.x version.
* LT-5890: Keep schema for appsettings.json up to date.


## 1.11.0 - Nova 2. Delivery 47 (November 15, 2024)
### What's changed
* LT-5844: Update messagepack to 2.x version.
* LT-5767: Add assembly load logger.


## 1.10.0 - Nova 2. Delivery 46 (September 27, 2024)
### What's changed
* LT-5674: - notifications logs version.
* LT-5629: Add central package management.


## 1.9.0 - Nova 2. Delivery 44 (August 16, 2024)
### What's changed
* LT-5622: Firebase not sending push notifications for mobile devices.
* LT-5521: Update rabbitmq broker library with new rabbitmq.client and templates.

### Deployment
Deployment of this service might require to delete `dev.NotificationsService.queue.MessagePreview` queue since its going to be no longer a durable queue.

In some cases, you may encounter an error indicating that the server-side configuration of a queue differs from the client’s expected configuration. If this occurs, please delete the queue, allowing it to be automatically recreated by the client.

Please be aware that the provided queue names may include environment-specific identifiers (e.g., dev, test, prod). Be sure to replace these with the actual environment name in use. The same applies to instance names embedded within the queue names (e.g., DefaultEnv, etc.).


## 1.8.0 - Nova 2. Delivery 43. Hotfix 2 (June 18, 2024)
### What's changed
* LT-5485: Migrate to net 8.

### Deployment
Since the default port in .NET 8 has been changed from 80 to 8080, to keep the previous behaviour you might need to change the port back. It can be achieved by using the following environment variable setup: `ASPNETCORE_HTTP_PORTS=80`.


## 1.7.0 - Nova 2. Delivery 41 (March 29, 2024)
### What's changed
* LT-5442: Update packages.




## 1.6.0 - Nova 2. Delivery 40 (February 28, 2024)
### What's changed
* LT-5285: Step: deprecated packages validation is failed.
* LT-5210: Update lykke.httpclientgenerator to 5.6.2.




## 1.5.0 - Nova 2. Delivery 39 (January 30, 2024)
### What's changed
* LT-5147: Changelog.md for lykke.snow.notifications.




## 1.4.2 - Nova 2. Delivery 38 (December 12, 2023)
### What's changed
* LT-5003: Push notification on-behalf.


**Full change log**: https://github.com/lykkebusiness/lykke.snow.notifications/compare/v1.3.1...v1.4.2

## 1.3.1 - Nova 2. Delivery 37 (2023-10-18)
### What's changed
* LT-5002: Add timestamp to device configuration tables.
* LT-4976: Update contract size constraint.
* LT-4934: Contract size is not taken into account.

### Deployment
* Add new configuration key `NotificationService.AssetServiceClient.ServiceUrl`
* Add new configuration key `NotificationService.AssetServiceClient.ApiKey`


**Full change log**: https://github.com/lykkebusiness/lykke.snow.notifications/compare/v1.2.1...v1.3.1


## 1.2.1 - Nova 2. Delivery 36 (2023-08-31)
### What's changed
* LT-4894: Update nugets.
* LT-4871: Notification issue log when starting and not show the version.


**Full change log**: https://github.com/lykkebusiness/lykke.snow.notifications/compare/v1.1.1...v1.2.1


## 1.1.1 - Nova 2. Delivery 34 (2023-06-05)
### What's changed
* LT-4758: Use AccountName instead of AccountId where applicable
* LT-4721: Mdm integration for push notification localization, to enable uploading files through FE
* LT-4720: Add localization for push notification parameters
* LT-4687: Send notifications for on-behalf actions
* LT-4646: Send notifications for Corporate Actions
* LT-4679: Add price alert notifications

### Deployment

**1. Mdm integration**

- Update the appsettings.json to include "MdmServiceClient" and "Localization" sections as shown in README.md file.

- Set a platform key for the localization file in appsettings.json as shown in README.md file. It must be 'PushNotifications' as this is the key Frontend recognizes.

- Upload the localization file with the specified platform key through the FE. (PlatformKey = 'PushNotifications')

**2. Localization for paramters**

- Update the localization file to include attribute translations as shown in README.md file.

- Update the appsettings.json to include 'TranslateAttributes' collection to define the list of attributes that are eligible for translation. These words are given below

- Order Direction. BUY and SELL
- Position direction - LONG and SHORT
- Order type - MARKET, LIMIT, STOP, TAKEPROFIT, STOPLOSS, TRAILINGSTOP

**3. On-Behalf actions**

There are four new notification types introduced with this change

- OnBehalfOrderPlacement
- OnBehalfOrderModification
- OnBehalfOrderCancellation
- OnBehalfPositionClosing

These notification types will be enabled for the newly registered devices. However, in order to enable these notification for existing devices, the following queries must be run.

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4687](https://lykke-snow.atlassian.net/browse/LT-4687)-enable-on-behalf-order-placed-notification.sql

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4687](https://lykke-snow.atlassian.net/browse/LT-4687)-enable-on-behalf-order-modification-notification.sql

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4687](https://lykke-snow.atlassian.net/browse/LT-4687)-enable-on-behalf-order-cancellation-notification.sql

-  src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4687](https://lykke-snow.atlassian.net/browse/LT-4687)-enable-on-behalf-position-closing-notification.sql

ROLLBACK SCRIPTS

1. Rollback enabled 'OnBehalfOrderPlacement' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'OnBehalfOrderPlacement'

2. Rollback enabled 'OnBehalfOrderModification' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'OnBehalfOrderModification'

3. Rollback enabled 'OnBehalfOrderCancellation' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'OnBehalfOrderCancellation'

4. Rollback enabled 'OnBehalfPositionClosing' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'OnBehalfPositionClosing'

Please, also provide translations for these new notification types in the localization file. `OnBehalfOrderPlacement`, `OnBehalfOrderModification`, `OnBehalfOrderCancellation`, and `OnBehalfPositionClosing`.


**4. Price Alert notifications**

For newly registered devices the Price Alert notifications are enabled as default.

For existing devices, the following script must be run.

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4680](https://lykke-snow.atlassian.net/browse/LT-4680)-enable-price-alerts-notifications.sql

ROLLBACK SCRIPT

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'PriceAlertTriggered'

Please, also provide the translation for the new notification type `PriceAlertTriggered` in the localization file.

**5. Corporate Actions notificaitons**

For newly registered devices; CorporateAction, PlatformHoliday, and MarketHoliday notifications will be enabled as default.

For already registered devices the notification must be enabled by running the following scripts.

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4646](https://lykke-snow.atlassian.net/browse/LT-4646)-enable-ca-notifications.sql

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4646](https://lykke-snow.atlassian.net/browse/LT-4646)-enable-platformholiday-notifications.sql

- src/Lykke.Snow.Notifications.SqlRepositories/Scripts/[LT-4646](https://lykke-snow.atlassian.net/browse/LT-4646)-enable-marketholiday-notifications.sql

Please, also provide translations for these new notification types in the localization file. `CAPositionAboutToClose`, `PlatformHoliday`, and `MarketHoliday`.

ROLLBACK SCRIPTS

1. Rollback enabled 'CorporateActions' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'CAPositionAboutToClose'

2. Rollback enabled 'PlatformHoliday' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'PlatformHoliday'

3. Rollback enabled 'MarketHoliday' notifications

DELETE from [notifications].DeviceNotificationConfigurations WHERE NotificationType = 'MarketHoliday'
      
**Full Changelog**: https://github.com/LykkeBusiness/Lykke.Snow.Notifications/compare/v1.0.3...v1.1.1


## 1.0.3 - Nova 2. Delivery 33. Hotfix 3 (2023-05-02)
### What's changed
- LT-4707: Embed Serilog configuration into appsettings.json

**Full Changelog**: https://github.com/LykkeBusiness/Lykke.Snow.Notifications/compare/v1.0.2...v1.0.3


## 1.0.2 - Nova 2. Delivery 33. Hotfix 2 (2023-04-28)
### What's changed
- LT-4705: Use proxy for Google Authentication requests

**Full Changelog**: https://github.com/LykkeBusiness/Lykke.Snow.Notifications/compare/v1.0.1...v1.0.2


## 1.0.1 - Nova 2. Delivery 33. Hotfix 1 (2023-04-28)
### What's changed
- LT-4691: Fix race condition on device registrations
- LT-4692: Enrich push notification logs to facilitate diagnosing

**Full Changelog**: https://github.com/LykkeBusiness/Lykke.Snow.Notifications/compare/v1.0.0...v1.0.1


## 1.0.0 - Nova 2. Delivery 33. (2023-04-11)
### What's changed
* LT-4648*: Add proxy for service.
* LT-4623: Push notifications localization.
* LT-4621: Implement on/off configuration.
* LT-4616*: Create notification models.
* LT-4604: Create endpoint to save device tokens.

### Deployment
* Configuration section "Proxy" with address, username and password has been added. Section is optional.
```json
{
    "Proxy": {
        "Address": "proxy_server_address",
        "UserName": "user",
        "Password": "password"
    }
}
```
* Configuration of MessagePreviewSubscriber:
  * New queue must be created (e.g. <env-name>.NotificationsService.queue.MessagePreview)
  * Binding has to be set upon routing key: "MessagePreview" from exchange lykke.meteor.messages.notifications.
* This is the first component release, all the configuration details not listed here can be found in `README.md` file.
