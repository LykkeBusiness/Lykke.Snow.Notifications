using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class UniqueIndexOnDeviceIdAndAccountId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_DeviceId_AccountId",
                schema: "notifications",
                table: "DeviceConfigurations",
                columns: new[] { "DeviceId", "AccountId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceConfigurations_DeviceId_AccountId",
                schema: "notifications",
                table: "DeviceConfigurations");
        }
    }
}
