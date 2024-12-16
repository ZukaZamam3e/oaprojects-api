﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OAProjects.Data.CatanLogger.Context;

#nullable disable

namespace OAProjects.Data.CatanLogger.Migrations
{
    [DbContext(typeof(CatanLoggerDbContext))]
    partial class CatanLoggerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_DICEROLL", b =>
                {
                    b.Property<int>("DICE_ROLL_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("DICE_ROLL_ID"));

                    b.Property<int>("DICE_NUMBER")
                        .HasColumnType("int");

                    b.Property<int>("DICE_ROLLS")
                        .HasColumnType("int");

                    b.Property<int>("GAME_ID")
                        .HasColumnType("int");

                    b.HasKey("DICE_ROLL_ID");

                    b.HasIndex("GAME_ID");

                    b.ToTable("CL_DICEROLL");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GAME", b =>
                {
                    b.Property<int>("GAME_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GAME_ID"));

                    b.Property<DateTime>("DATE_PLAYED")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("GAME_DELETED")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("GROUP_ID")
                        .HasColumnType("int");

                    b.Property<string>("TURN_ORDER")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("GAME_ID");

                    b.HasIndex("GROUP_ID");

                    b.ToTable("CL_GAME");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GROUP", b =>
                {
                    b.Property<int>("GROUP_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GROUP_ID"));

                    b.Property<DateTime>("DATE_ADDED")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("GROUP_NAME")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("GROUP_ID");

                    b.ToTable("CL_GROUP");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GROUP_USER", b =>
                {
                    b.Property<int>("GROUP_USER_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("GROUP_USER_ID"));

                    b.Property<int>("CONFIRMED_USER_ID")
                        .HasColumnType("int");

                    b.Property<DateTime>("DATE_ADDED")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("GROUP_ID")
                        .HasColumnType("int");

                    b.Property<int>("GROUP_USER_STATUS")
                        .HasColumnType("int");

                    b.Property<int>("ROLE_ID")
                        .HasColumnType("int");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("GROUP_USER_ID");

                    b.HasIndex("GROUP_ID");

                    b.ToTable("CL_GROUP_USER");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_PLAYER", b =>
                {
                    b.Property<int>("PLAYER_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("PLAYER_ID"));

                    b.Property<int>("GAME_ID")
                        .HasColumnType("int");

                    b.Property<bool>("IS_PLAYING")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("PLAYER_COLOR")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PLAYER_NAME")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<bool>("WINNER")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("PLAYER_ID");

                    b.HasIndex("GAME_ID");

                    b.ToTable("CL_PLAYER");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_DICEROLL", b =>
                {
                    b.HasOne("OAProjects.Data.CatanLogger.Entities.CL_GAME", "GAME")
                        .WithMany("DICE_ROLLS")
                        .HasForeignKey("GAME_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GAME");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GAME", b =>
                {
                    b.HasOne("OAProjects.Data.CatanLogger.Entities.CL_GROUP", "GROUP")
                        .WithMany("GAMES")
                        .HasForeignKey("GROUP_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GROUP");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GROUP_USER", b =>
                {
                    b.HasOne("OAProjects.Data.CatanLogger.Entities.CL_GROUP", "GROUP")
                        .WithMany("GROUP_USERS")
                        .HasForeignKey("GROUP_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GROUP");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_PLAYER", b =>
                {
                    b.HasOne("OAProjects.Data.CatanLogger.Entities.CL_GAME", "GAME")
                        .WithMany("PLAYERS")
                        .HasForeignKey("GAME_ID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GAME");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GAME", b =>
                {
                    b.Navigation("DICE_ROLLS");

                    b.Navigation("PLAYERS");
                });

            modelBuilder.Entity("OAProjects.Data.CatanLogger.Entities.CL_GROUP", b =>
                {
                    b.Navigation("GAMES");

                    b.Navigation("GROUP_USERS");
                });
#pragma warning restore 612, 618
        }
    }
}
