﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SupplyNest.Inventory.Api.Infrastructure.SqlServerConfig.DbContexts;

#nullable disable

namespace SupplyNest.Inventory.Api.Infrastructure.Migrations
{
    [DbContext(typeof(InventoryDbContext))]
    partial class InventoryDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);
 
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SupplyNest.Inventory.Api.Domain.Entities.Inventory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAtTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("FiscalYearId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("LastModifiedTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<Guid>("ProductId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("WarehouseId")
                        .HasColumnType("uuid");

                    b.ComplexProperty<Dictionary<string, object>>("CurrentInventoryQuantity", "SupplyNest.Inventory.Api.Domain.Entities.Inventory.CurrentInventoryQuantity#InventoryQuantity", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<long>("Value")
                                .HasColumnType("bigint")
                                .HasColumnName("CurrentInventoryQuantity");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("CurrentSaleQuantity", "SupplyNest.Inventory.Api.Domain.Entities.Inventory.CurrentSaleQuantity#InventoryQuantity", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<long>("Value")
                                .HasColumnType("bigint")
                                .HasColumnName("CurrentSaleQuantity");
                        });

                    b.HasKey("Id");

                    b.HasIndex("FiscalYearId");

                    b.HasIndex("ProductId");

                    b.HasIndex("WarehouseId");

                    b.HasIndex("WarehouseId", "ProductId", "FiscalYearId")
                        .IsUnique();

                    b.ToTable("Inventories", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
