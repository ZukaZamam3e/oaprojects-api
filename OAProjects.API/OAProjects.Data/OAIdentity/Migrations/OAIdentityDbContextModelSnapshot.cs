﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OAProjects.Data.OAIdentity.Context;

#nullable disable

namespace OAProjects.Data.OAIdentity.Migrations
{
    [DbContext(typeof(OAIdentityDbContext))]
    partial class OAIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("OAProjects.Data.OAIdentity.Entities.OA_ID_XREF", b =>
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

                    b.ToTable("OA_ID_XREF");
                });

            modelBuilder.Entity("OAProjects.Data.OAIdentity.Entities.OA_USER", b =>
                {
                    b.Property<int>("USER_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("USER_ID"), 1L, 1);

                    b.Property<DateTime>("DATE_ADDED")
                        .HasColumnType("datetime2");

                    b.Property<string>("EMAIL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FIRST_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LAST_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("USER_NAME")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("USER_ID");

                    b.ToTable("OA_USER");
                });

            modelBuilder.Entity("OAProjects.Data.OAIdentity.Entities.OA_USER_TOKEN", b =>
                {
                    b.Property<int>("USER_TOKEN_ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("USER_TOKEN_ID"), 1L, 1);

                    b.Property<DateTime>("EXPIRY_DATE_UTC")
                        .HasColumnType("datetime2");

                    b.Property<int>("EXPIRY_TIME")
                        .HasColumnType("int");

                    b.Property<int>("ISSUED_AT")
                        .HasColumnType("int");

                    b.Property<DateTime>("ISSUED_AT_DATE_UTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("TOKEN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("USER_ID")
                        .HasColumnType("int");

                    b.HasKey("USER_TOKEN_ID");

                    b.ToTable("OA_USER_TOKEN");
                });
#pragma warning restore 612, 618
        }
    }
}
