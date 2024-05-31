﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OAProjects.Data.ShowLogger.Context;

#nullable disable

namespace OAProjects.Data.ShowLogger.Migrations
{
    [DbContext(typeof(ShowLoggerDbContext))]
    [Migration("20240522025027_0.015")]
    partial class _0015
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_BOOK", b =>
                {
                    b.Property<int>("BOOK_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BOOK_ID"), 1L, 1);

                    b.Property<string>("BOOK_NAME")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("BOOK_NOTES")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int?>("CHAPTERS")
                        .HasColumnType("int");

                    b.Property<DateTime?>("END_DATE")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PAGES")
                        .HasColumnType("int");

                    b.Property<DateTime?>("START_DATE")
                        .HasColumnType("datetime2");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("BOOK_ID");

                    b.ToTable("SL_BOOK");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_CODE_VALUE", b =>
                {
                    b.Property<int>("CODE_VALUE_ID")
                        .HasColumnType("int");

                    b.Property<int>("CODE_TABLE_ID")
                        .HasColumnType("int");

                    b.Property<string>("DECODE_TXT")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EXTRA_INFO")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("CODE_VALUE_ID");

                    b.ToTable("SL_CODE_VALUE");

                    b.HasData(
                        new
                        {
                            CODE_VALUE_ID = 1000,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "TV"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1001,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "Movie"
                        },
                        new
                        {
                            CODE_VALUE_ID = 1002,
                            CODE_TABLE_ID = 1,
                            DECODE_TXT = "AMC"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2000,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "A-list Ticket"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2001,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "Ticket"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2002,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "Purchase"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2003,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "AMC A-list"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2004,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "Benefits"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2005,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "Rewards"
                        },
                        new
                        {
                            CODE_VALUE_ID = 2006,
                            CODE_TABLE_ID = 2,
                            DECODE_TXT = "Tax"
                        });
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_FRIEND", b =>
                {
                    b.Property<int>("FRIEND_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FRIEND_ID"), 1L, 1);

                    b.Property<DateTime>("CREATED_DATE")
                        .HasColumnType("datetime2");

                    b.Property<int>("FRIEND_USER_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("FRIEND_ID");

                    b.ToTable("SL_FRIEND");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_FRIEND_REQUEST", b =>
                {
                    b.Property<int>("FRIEND_REQUEST_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FRIEND_REQUEST_ID"), 1L, 1);

                    b.Property<DateTime>("DATE_SENT")
                        .HasColumnType("datetime2");

                    b.Property<int>("RECEIVED_USER_ID")
                        .HasColumnType("int");

                    b.Property<int>("SENT_USER_ID")
                        .HasColumnType("int");

                    b.HasKey("FRIEND_REQUEST_ID");

                    b.ToTable("SL_FRIEND_REQUEST");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_ID_XREF", b =>
                {
                    b.Property<int>("ID_XREF_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID_XREF_ID"), 1L, 1);

                    b.Property<int>("NEW_ID")
                        .HasColumnType("int");

                    b.Property<int>("OLD_ID")
                        .HasColumnType("int");

                    b.Property<int>("TABLE_ID")
                        .HasColumnType("int");

                    b.HasKey("ID_XREF_ID");

                    b.ToTable("SL_ID_XREF");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_MOVIE_INFO", b =>
                {
                    b.Property<int>("MOVIE_INFO_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("MOVIE_INFO_ID"), 1L, 1);

                    b.Property<DateTime?>("AIR_DATE")
                        .HasColumnType("datetime2");

                    b.Property<string>("API_ID")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int?>("API_TYPE")
                        .HasColumnType("int");

                    b.Property<string>("BACKDROP_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LAST_DATA_REFRESH")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LAST_UPDATED")
                        .HasColumnType("datetime2");

                    b.Property<string>("MOVIE_NAME")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<string>("MOVIE_OVERVIEW")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("POSTER_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RUNTIME")
                        .HasColumnType("int");

                    b.HasKey("MOVIE_INFO_ID");

                    b.ToTable("SL_MOVIE_INFO");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_SHOW", b =>
                {
                    b.Property<int>("SHOW_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SHOW_ID"), 1L, 1);

                    b.Property<DateTime>("DATE_WATCHED")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EPISODE_NUMBER")
                        .HasColumnType("int");

                    b.Property<int?>("INFO_ID")
                        .HasColumnType("int");

                    b.Property<bool>("RESTART_BINGE")
                        .HasColumnType("bit");

                    b.Property<int?>("SEASON_NUMBER")
                        .HasColumnType("int");

                    b.Property<string>("SHOW_NAME")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SHOW_NOTES")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("SHOW_TYPE_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("SHOW_ID");

                    b.ToTable("SL_SHOW");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TRANSACTION", b =>
                {
                    b.Property<int>("TRANSACTION_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TRANSACTION_ID"), 1L, 1);

                    b.Property<decimal>("COST_AMT")
                        .HasPrecision(6, 2)
                        .HasColumnType("decimal(6,2)");

                    b.Property<string>("ITEM")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("QUANTITY")
                        .HasColumnType("int");

                    b.Property<int?>("SHOW_ID")
                        .HasColumnType("int");

                    b.Property<DateTime>("TRANSACTION_DATE")
                        .HasColumnType("datetime2");

                    b.Property<string>("TRANSACTION_NOTES")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("TRANSACTION_TYPE_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("TRANSACTION_ID");

                    b.ToTable("SL_TRANSACTION");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TV_EPISODE_INFO", b =>
                {
                    b.Property<int>("TV_EPISODE_INFO_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TV_EPISODE_INFO_ID"), 1L, 1);

                    b.Property<DateTime?>("AIR_DATE")
                        .HasColumnType("datetime2");

                    b.Property<string>("API_ID")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int?>("API_TYPE")
                        .HasColumnType("int");

                    b.Property<string>("EPISODE_NAME")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.Property<int?>("EPISODE_NUMBER")
                        .HasColumnType("int");

                    b.Property<string>("EPISODE_OVERVIEW")
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("IMAGE_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("RUNTIME")
                        .HasColumnType("int");

                    b.Property<string>("SEASON_NAME")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("SEASON_NUMBER")
                        .HasColumnType("int");

                    b.Property<int>("TV_INFO_ID")
                        .HasColumnType("int");

                    b.HasKey("TV_EPISODE_INFO_ID");

                    b.HasIndex("TV_INFO_ID");

                    b.ToTable("SL_TV_EPISODE_INFO");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TV_EPISODE_ORDER", b =>
                {
                    b.Property<int>("TV_EPISODE_ORDER_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TV_EPISODE_ORDER_ID"), 1L, 1);

                    b.Property<int>("EPISODE_ORDER")
                        .HasColumnType("int");

                    b.Property<int>("TV_EPISODE_INFO_ID")
                        .HasColumnType("int");

                    b.Property<int>("TV_INFO_ID")
                        .HasColumnType("int");

                    b.HasKey("TV_EPISODE_ORDER_ID");

                    b.ToTable("SL_TV_EPISODE_ORDER");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TV_INFO", b =>
                {
                    b.Property<int>("TV_INFO_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TV_INFO_ID"), 1L, 1);

                    b.Property<string>("API_ID")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.Property<int?>("API_TYPE")
                        .HasColumnType("int");

                    b.Property<string>("BACKDROP_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LAST_DATA_REFRESH")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LAST_UPDATED")
                        .HasColumnType("datetime2");

                    b.Property<string>("POSTER_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SHOW_NAME")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SHOW_OVERVIEW")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("STATUS")
                        .HasMaxLength(25)
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("TV_INFO_ID");

                    b.ToTable("SL_TV_INFO");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_USER_PREF", b =>
                {
                    b.Property<int>("USER_PREF_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("USER_PREF_ID"), 1L, 1);

                    b.Property<string>("DEFAULT_AREA")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("USER_PREF_ID");

                    b.ToTable("SL_USER_PREF");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_WATCHLIST", b =>
                {
                    b.Property<int>("WATCHLIST_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("WATCHLIST_ID"), 1L, 1);

                    b.Property<DateTime>("DATE_ADDED")
                        .HasColumnType("datetime2");

                    b.Property<int?>("EPISODE_NUMBER")
                        .HasColumnType("int");

                    b.Property<int?>("INFO_ID")
                        .HasColumnType("int");

                    b.Property<int?>("SEASON_NUMBER")
                        .HasColumnType("int");

                    b.Property<string>("SHOW_NAME")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("SHOW_NOTES")
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("SHOW_TYPE_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("WATCHLIST_ID");

                    b.ToTable("SL_WATCHLIST");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Views.SL_YEAR_STATS_DATA_VW", b =>
                {
                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.Property<int>("YEAR")
                        .HasColumnType("int");

                    b.Property<string>("SHOW_NAME")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("API_ID")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("API_TYPE")
                        .HasColumnType("int");

                    b.Property<string>("BACKDROP_URL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SHOW_TYPE_ID")
                        .HasColumnType("int");

                    b.Property<int?>("TOTAL_RUNTIME")
                        .HasColumnType("int");

                    b.Property<int>("WATCH_COUNT")
                        .HasColumnType("int");

                    b.HasKey("USER_ID", "YEAR", "SHOW_NAME");

                    b.ToView("SL_YEAR_STATS_DATA_VW");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TV_EPISODE_INFO", b =>
                {
                    b.HasOne("OAProjects.Data.ShowLogger.Entities.SL_TV_INFO", "TV_INFO")
                        .WithMany("EPISODE_INFOS")
                        .HasForeignKey("TV_INFO_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TV_INFO");
                });

            modelBuilder.Entity("OAProjects.Data.ShowLogger.Entities.SL_TV_INFO", b =>
                {
                    b.Navigation("EPISODE_INFOS");
                });
#pragma warning restore 612, 618
        }
    }
}
