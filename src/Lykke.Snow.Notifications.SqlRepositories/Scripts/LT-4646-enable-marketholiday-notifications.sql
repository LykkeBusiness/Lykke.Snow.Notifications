-- Enable market holiday notifications for existing devices.

DECLARE @NotificationType NVARCHAR(255) = 'MarketHoliday'; 

INSERT INTO notifications.DeviceNotificationConfigurations 
(NotificationType, Enabled, DeviceConfigurationId)
SELECT @NotificationType, 1, DeviceConfigurationId
FROM (
    SELECT DISTINCT DeviceConfigurationId
    FROM notifications.DeviceNotificationConfigurations
) AS Subquery
WHERE NOT EXISTS (
    SELECT 1
    FROM notifications.DeviceNotificationConfigurations
    WHERE NotificationType = @NotificationType
    AND DeviceConfigurationId = Subquery.DeviceConfigurationId
)