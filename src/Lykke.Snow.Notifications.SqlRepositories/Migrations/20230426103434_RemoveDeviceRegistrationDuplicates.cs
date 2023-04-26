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
DELETE FROM DuplicateRecords
WHERE RowNumber > 1;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
