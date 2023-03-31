# Lykke.Snow.Notifications
This project sends notifications about notable events across the Nova Platform to interested parties.
This service consumes events from the Activities producer and becomes aware of notable events

## How to use in prod environment?
// To be filled.

## How to run for debug?

1. Clone the repository anwhere in filesystem.
2. Create an `appsettings.dev.json` file in `src/Lykke.Snow.Notifications` directory. (Please refer to **Configuration** section.)
3. Add an environment variable `SettingsUrl` with value of `appsettings.dev.json`
4. Setup firebase service account credentials (please refer to configuration section)
5. Corresponding VPN (dev, test) must be activated prior to running the project.
6. Run the project.

## Dependencies

- MSSQL
- RabbitMQ broker
- Firebase Cloud Messaging.

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

In order to communicate with Firebase servers, a private key for service account is needed. This file should be located in output directory. Please follow the steps [here](https://firebase.google.com/docs/admin/setup) to see get this credentials file.

After getting this file and moving it to the output directory, the `CredentialsFilePath` configuration should be set to point the path in output directory.

```json
"Fcm":{
	"CredentialFilePath": "path-to-the-file.json"
}
```

### 3. Localization

A localization in JSON format file should be provided in order to render notification messages in preferred language. That file can be found at the root directory. ([localization.json](./src/Lykke.Snow.Notifications/localization.json))

The file has the following format.

```json
{
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
> That means notification type names in this file exactly match with the notification type.
And the languages should be put in lowercase format. (i.e. `en`, `es`, `de`)

All notification types can be found at [here](./src/Lykke.Snow.Notifications.Domain/Enums/NotificationType.cs). 


### Environment variables

* *RESTART_ATTEMPTS_NUMBER* - number of restart attempts. If not set int.MaxValue is used.
* *RESTART_ATTEMPTS_INTERVAL_MS* - interval between restarts in milliseconds. If not set 10000 is used.
* *SettingsUrl* - defines URL of remote settings or path for local settings.

### Settings 

Settings schema is

```json
{
  "NotificationService": 
  {
    "Db": 
    {
      "ConnectionString": ""
    },
    "Cqrs": 
    {
      "ConnectionString": "",
      "RetryDelay": "00:00:02",
      "EnvironmentName": ""
    },
    "Fcm": 
    {
      "CredentialFilePath": ""
    },
    "NotificationServiceClient": 
    {
      "Url": "",
      "ApiKey": ""
    },
    "Subscribers": 
    {
      "MessagePreviewSubscriber": 
      {
        "ConnectionString": "",
        "QueueName": "",
        "RoutingKey": "",
        "ExchangeName": "",
        "IsDurable": true
      }
    }
  }
}
```