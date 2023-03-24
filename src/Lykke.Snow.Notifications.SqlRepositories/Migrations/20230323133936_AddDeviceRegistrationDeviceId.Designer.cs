﻿// <auto-generated />
using System;
using Lykke.Snow.Notifications.SqlRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Lykke.Snow.Notifications.SqlRepositories.Migrations
{
    [DbContext(typeof(NotificationsDbContext))]
    [Migration("20230323133936_AddDeviceRegistrationDeviceId")]
    partial class AddDeviceRegistrationDeviceId
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("notifications")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceConfigurationEntity", b =>
                {
                    b.Property<int>("Oid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Oid"), 1L, 1);

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("CreatedOn")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("Locale")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.HasKey("Oid");

                    b.HasIndex("DeviceId")
                        .IsUnique();

                    b.ToTable("DeviceConfigurations", "notifications");
                });

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceNotificationConfigurationEntity", b =>
                {
                    b.Property<int>("Oid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Oid"), 1L, 1);

                    b.Property<int>("DeviceConfigurationId")
                        .HasColumnType("int");

                    b.Property<bool>("Enabled")
                        .HasColumnType("bit");

                    b.Property<string>("NotificationType")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Oid");

                    b.HasIndex("DeviceConfigurationId", "NotificationType")
                        .IsUnique();

                    b.ToTable("DeviceNotificationConfigurations", "notifications");
                });

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceRegistrationEntity", b =>
                {
                    b.Property<int>("Oid")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Oid"), 1L, 1);

                    b.Property<string>("AccountId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("DeviceId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("DeviceToken")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<DateTime>("RegisteredOn")
                        .HasColumnType("datetime2");

                    b.HasKey("Oid");

                    b.HasIndex("DeviceToken")
                        .IsUnique();

                    b.ToTable("DeviceRegistrations", "notifications");
                });

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceNotificationConfigurationEntity", b =>
                {
                    b.HasOne("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceConfigurationEntity", "DeviceConfiguration")
                        .WithMany("Notifications")
                        .HasForeignKey("DeviceConfigurationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeviceConfiguration");
                });

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceConfigurationEntity", b =>
                {
                    b.Navigation("Notifications");
                });
#pragma warning restore 612, 618
        }
    }
}
