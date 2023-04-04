using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class RemoveUniqueConstraint : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceRegistrations_DeviceToken",
                schema: "notifications",
                table: "DeviceRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_DeviceId",
                schema: "notifications",
                table: "DeviceRegistrations",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_DeviceToken",
                schema: "notifications",
                table: "DeviceRegistrations",
                column: "DeviceToken");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceRegistrations_DeviceId",
                schema: "notifications",
                table: "DeviceRegistrations");

            migrationBuilder.DropIndex(
                name: "IX_DeviceRegistrations_DeviceToken",
                schema: "notifications",
                table: "DeviceRegistrations");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_DeviceToken",
                schema: "notifications",
                table: "DeviceRegistrations",
                column: "DeviceToken",
                unique: true);
        }
    }
}
