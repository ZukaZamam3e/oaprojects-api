﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OAProjects.Data.FinanceTracker.Context;

#nullable disable

namespace OAProjects.Data.FinanceTracker.Migrations
{
    [DbContext(typeof(FinanceTrackerDbContext))]
    [Migration("20241024014259_0.003")]
    partial class _0003
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("OAProjects.Data.FinanceTracker.Entities.FT_ACCOUNT", b =>
                {
                    b.Property<int>("ACCOUNT_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ACCOUNT_ID"));

                    b.Property<string>("ACCOUNT_NAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<bool>("DEFAULT_INDC")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("ACCOUNT_ID");

                    b.ToTable("FT_ACCOUNT");
                });

            modelBuilder.Entity("OAProjects.Data.FinanceTracker.Entities.FT_CODE_VALUE", b =>
                {
                    b.Property<int>("CODE_VALUE_ID")
                        .HasColumnType("int");

                    b.Property<int>("CODE_TABLE_ID")
                        .HasColumnType("int");

                    b.Property<string>("DECODE_TXT")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("EXTRA_INFO")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.HasKey("CODE_VALUE_ID");

                    b.ToTable("FT_CODE_VALUE");

                    b.HasData(
                        new
                        {
                            CODE_VALUE_ID = 1000,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Hardset"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1001,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Single"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1002,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Daily"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1003,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Weekly"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1004,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Bi-Weekly"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1005,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Every Four Weeks"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1006,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Monthly"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1007,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Quarterly"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1008,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Yearly"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1009,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Every N Days"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1010,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Every N Weeks"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1011,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Every N Months"
                        });
                });

            modelBuilder.Entity("OAProjects.Data.FinanceTracker.Entities.FT_ID_XREF", b =>
                {
                    b.Property<int>("ID_XREF_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("ID_XREF_ID"));

                    b.Property<int>("NEW_ID")
                        .HasColumnType("int");

                    b.Property<int>("OLD_ID")
                        .HasColumnType("int");

                    b.Property<int>("TABLE_ID")
                        .HasColumnType("int");

                    b.HasKey("ID_XREF_ID");

                    b.ToTable("FT_ID_XREF");
                });

            modelBuilder.Entity("OAProjects.Data.FinanceTracker.Entities.FT_TRANSACTION", b =>
                {
                    b.Property<int>("TRANSACTION_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TRANSACTION_ID"));

                    b.Property<int>("ACCOUNT_ID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("END_DATE")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("FREQUENCY_TYPE_ID")
                        .HasColumnType("int");

                    b.Property<int?>("INTERVAL")
                        .HasColumnType("int");

                    b.Property<DateTime>("START_DATE")
                        .HasColumnType("datetime(6)");

                    b.Property<decimal>("TRANSACTION_AMOUNT")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("TRANSACTION_NAME")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("TRANSACTION_NOTES")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<string>("TRANSACTION_URL")
                        .HasMaxLength(250)
                        .HasColumnType("varchar(250)");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("TRANSACTION_ID");

                    b.ToTable("FT_TRANSACTION");
                });

            modelBuilder.Entity("OAProjects.Data.FinanceTracker.Entities.FT_TRANSACTION_OFFSET", b =>
                {
                    b.Property<int>("TRANSACTION_OFFSET_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("TRANSACTION_OFFSET_ID"));

                    b.Property<int>("ACCOUNT_ID")
                        .HasColumnType("int");

                    b.Property<decimal>("OFFSET_AMOUNT")
                        .HasColumnType("decimal(65,30)");

                    b.Property<DateTime>("OFFSET_DATE")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TRANSACTION_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("TRANSACTION_OFFSET_ID");

                    b.ToTable("FT_TRANSACTION_OFFSET");
                });
#pragma warning restore 612, 618
        }
    }
}
