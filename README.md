# Lykke.Snow.Notifications
This project sends notifications about notable events across the Nova Platform to interested parties.
This service consumes events from the Activities producer and becomes aware of notable events

## How to use in prod environment?
// To be filled.

## How to run for debug?

1. Clone the repository anywhere in filesystem.
2. Create an `appsettings.dev.json` file in `src/Lykke.Snow.Notifications` directory. (Please refer to **Configuration** section.)
3. Add an environment variable `SettingsUrl` with value of `appsettings.dev.json`
4. Setup firebase service account credentials (please refer to configuration section)
5. Corresponding VPN (dev, test) must be activated prior to running the project.
6. Run the project.

## Dependencies

- MSSQL
- RabbitMQ broker
- Firebase Cloud Messaging.
- Mdm service

## Configuration

### 1. Hosting Configuration

Kestrel configuration may be passed through appsettings.json, secrets or environment.
All variables and value constraints are default. For instance, to set host URL the following env variable may be set:

```json
{
    "Kestrel__EndPoints__Http__Url": "http://*:5010"
}
```
### 2. Firebase Configuration

In order to communicate with Firebase servers, a private key for service account is needed. This file should be located in output directory. Please follow the steps [here](https://firebase.google.com/docs/admin/setup) to get this credentials file.

After getting this file and moving it to the output directory, the `CredentialsFilePath` configuration should be set to point the path in output directory.

```json
"Fcm": {
  "CredentialFilePath": "/app/firebase-credentials.json"
}
```

### 3. Localization

A localization in JSON format file should be provided in order to render notification messages in preferred language. That file can be found at the root directory. ([localization.json](./src/Lykke.Snow.Notifications/localization.json))

The file has the following format.

```json
{
  "Attributes": {
    "BUY": {
      "en": "Buy",
      "es": "Comprar",
      "de": "Kauf"
    },
    "SELL": {
      "en": "Sell",
      "es": "Vender",
      "de": "Verkauf"
    }
  },
  "Titles": {
    "AccountLocked": {
      "en": "English text for notification title with {0} placeholders {1}.",
      "es": "Spanish translation for notificaiton title with {0} placeholders {1}.",
      "de": "German translation for notification title with {0} placeholders {1}.",
    }
  },
  "Bodies": {
    "AccountLocked": {
      "en": "English text for notification body with {0} placeholders {1}.",
      "es": "Spanish translation for notificaiton body with {0} placeholders {1}.",
      "de": "German translation for notification body with {0} placeholders {1}.",
    }
  }
}
```

There has to be corresponding object both in `Titles` and `Bodies` object for each notification type.

> ⚠️Please note that matching engine is case-sensitive! 
> That means notification type names in this file must have exact match with the notification types resides in the following file.
And the languages should be put in lowercase format. (i.e. `en`, `es`, `de`)

All notification types can be found at [here](./src/Lykke.Snow.Notifications.Domain/Enums/NotificationType.cs). 


### 3.1. Mdm Service Integration

Starting from 34th release, localization files are loaded from Mdm service.

In order to specify the platform key for the file that's been uploaded through Mdm Service, use the **LocalizationPlatformKey** configuration in the following configuration section.

Localization files are cached in Notification Service for given amount of time. This period is 5 minutes as default, and can be adjusted within the same configuration section.

```json
{
  "Localization": {
    "LocalizationPlatformKey": "PushNotification",
    "LocalizationFileCache": {
      "ExpirationPeriod": "00:01:00"
    },
    "TranslateAttributes": ["BUY", "SELL", "LONG", "SHORT"]
  }
}
```

### 3.2. Translating Attributes

Starting from 34th release, attributes are translated. (e.g. `BUY`, `SELL`, `STOPLOSS`, `SHORT`).

Please use the `Attributes` section in localization file to provide translation per language for individual attributes.

To make them eligible for translation, it's required to define them in `TranslateAttributes` section in `appsettings.json`. The attributes that's not defined in `TranslateAttributes` section won't be translated.

### 4. Proxy

Proxy is optional. Being set, it will be used for all FCM outgoing requests.

```json
{
    "Proxy": {
        "Address": "proxy.lykke.com",
        "UserName": "user",
        "Password": "password"
    }
}
```

### 5. Caching

Device configurations are cached in memory. The cache is invalidated on any change in the database. Additionally, there is an expiration time for the cache. The default value is 10 minutes but it can be changed in the configuration file.

```json
{
  "ConfigurationCache": {
    "ExpirationPeriod": "00:01:00"
  }
}
```

### 6. External Services

1. Mdm Service

Please specify the base url and api key for Mdm Service within the following configuration section.

```json
{
  "MdmServiceClient": {
    "ServiceUrl": "",
    "ApiKey": ""
  }
}
```

### Environment variables

* *RESTART_ATTEMPTS_NUMBER* - number of restart attempts. If not set int.MaxValue is used.
* *RESTART_ATTEMPTS_INTERVAL_MS* - interval between restarts in milliseconds. If not set 10000 is used.
* *SettingsUrl* - defines URL of remote settings or path for local settings.

### Settings 

Settings schema is:
<!-- MARKDOWN-AUTO-DOCS:START (CODE:src=./template.json) -->
<!-- MARKDOWN-AUTO-DOCS:END -->
