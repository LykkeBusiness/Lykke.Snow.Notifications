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
    [Migration("20230317135600_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("notifications")
                .HasAnnotation("ProductVersion", "6.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Lykke.Snow.Notifications.SqlRepositories.Entities.DeviceRegistrationEntity", b =>
                {
                    b.Property<string>("DeviceToken")
                        .HasMaxLength(512)
                        .HasColumnType("nvarchar(512)");

                    b.Property<string>("ClientId")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<DateTime>("RegisteredOn")
                        .HasColumnType("datetime");

                    b.HasKey("DeviceToken");

                    b.ToTable("DeviceRegistrations", "notifications");
                });
#pragma warning restore 612, 618
        }
    }
}