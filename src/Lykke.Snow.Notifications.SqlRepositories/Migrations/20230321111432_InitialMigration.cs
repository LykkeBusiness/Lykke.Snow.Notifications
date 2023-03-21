using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notifications");

            migrationBuilder.CreateTable(
                name: "DeviceRegistrations",
                schema: "notifications",
                columns: table => new
                {
                    Oid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    DeviceToken = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    RegisteredOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceRegistrations", x => x.Oid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeviceRegistrations_DeviceToken",
                schema: "notifications",
                table: "DeviceRegistrations",
                column: "DeviceToken",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceRegistrations",
                schema: "notifications");
        }
    }
}
