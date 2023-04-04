using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class RemoveUniqueConstraint1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceConfigurations_DeviceId",
                schema: "notifications",
                table: "DeviceConfigurations");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_DeviceId",
                schema: "notifications",
                table: "DeviceConfigurations",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DeviceConfigurations_DeviceId",
                schema: "notifications",
                table: "DeviceConfigurations");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_DeviceId",
                schema: "notifications",
                table: "DeviceConfigurations",
                column: "DeviceId",
                unique: true);
        }
    }
}
