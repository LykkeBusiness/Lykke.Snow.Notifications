using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class AddNotificationsConfiguration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceConfigurations",
                schema: "notifications",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceConfigurations", x => x.Oid);
                });

            migrationBuilder.CreateTable(
                name: "DeviceNotificationConfigurations",
                schema: "notifications",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationType = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    DeviceConfigurationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceNotificationConfigurations", x => x.Oid);
                    table.ForeignKey(
                        name: "FK_DeviceNotificationConfigurations_DeviceConfigurations_DeviceConfigurationId",
                        column: x => x.DeviceConfigurationId,
                        principalSchema: "notifications",
                        principalTable: "DeviceConfigurations",
                        principalColumn: "Oid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceConfigurations_DeviceId",
                schema: "notifications",
                table: "DeviceConfigurations",
                column: "DeviceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeviceNotificationConfigurations_DeviceConfigurationId_NotificationType",
                schema: "notifications",
                table: "DeviceNotificationConfigurations",
                columns: new[] { "DeviceConfigurationId", "NotificationType" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceNotificationConfigurations",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "DeviceConfigurations",
                schema: "notifications");
        }
    }
}
