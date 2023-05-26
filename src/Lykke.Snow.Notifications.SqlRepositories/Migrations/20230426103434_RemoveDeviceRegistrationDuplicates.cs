using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class RemoveDeviceRegistrationDuplicates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
WITH DuplicateRecords AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY AccountId, DeviceToken ORDER BY RegisteredOn DESC) AS RowNumber
    FROM notifications.DeviceRegistrations
)
-- Delete related entries in DeviceNotificationConfigurations
DELETE FROM notifications.DeviceNotificationConfigurations
WHERE DeviceConfigurationId IN (
    SELECT DC.Oid
    FROM notifications.DeviceConfigurations AS DC
    INNER JOIN DuplicateRecords AS DR ON DR.DeviceId = DC.DeviceId
    WHERE DR.RowNumber > 1
);

WITH DuplicateRecords AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY AccountId, DeviceToken ORDER BY RegisteredOn DESC) AS RowNumber
    FROM notifications.DeviceRegistrations
)
-- Delete related entries in DeviceConfigurations
DELETE FROM notifications.DeviceConfigurations
WHERE DeviceId IN (
    SELECT DeviceId
    FROM DuplicateRecords
    WHERE RowNumber > 1
);

WITH DuplicateRecords AS (
    SELECT *,
           ROW_NUMBER() OVER (PARTITION BY AccountId, DeviceToken ORDER BY RegisteredOn DESC) AS RowNumber
    FROM notifications.DeviceRegistrations
)
-- Delete duplicates from DeviceRegistrations
DELETE FROM DuplicateRecords
WHERE RowNumber > 1;
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
