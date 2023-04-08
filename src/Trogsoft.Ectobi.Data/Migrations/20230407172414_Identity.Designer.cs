﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Trogsoft.Ectobi.Data;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    [DbContext(typeof(EctoDb))]
    [Migration("20230407172414_Identity")]
    partial class Identity
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Batch", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Flags")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("SchemaId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchemaVersionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SchemaId");

                    b.HasIndex("SchemaVersionId");

                    b.ToTable("Batches");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.EctoRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.EctoUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.LookupSet", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("LookupSets");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.LookupSetValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("LookupSetId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NumericValue")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LookupSetId");

                    b.ToTable("LookupSetValues");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Period", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SchemaVersionId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("StartDateUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SchemaVersionId");

                    b.ToTable("Periods");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Populator", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Populators");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Process", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Processes");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.ProcessElement", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ProcessId")
                        .HasColumnType("bigint");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ProcessId");

                    b.ToTable("ProcessElements");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Record", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<long>("BatchId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BatchId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Schema", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Schemas");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaField", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("Flags")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SchemaId")
                        .HasColumnType("bigint");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchemaId");

                    b.ToTable("SchemaFields");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaFieldVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<int>("Flags")
                        .HasColumnType("int");

                    b.Property<long?>("LookupSetId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PopulatorId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ProcessId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchemaFieldId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchemaVersionId")
                        .HasColumnType("bigint");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LookupSetId");

                    b.HasIndex("PopulatorId");

                    b.HasIndex("ProcessId");

                    b.HasIndex("SchemaFieldId");

                    b.HasIndex("SchemaVersionId");

                    b.ToTable("SchemaFieldVersions");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaVersion", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SchemaId")
                        .HasColumnType("bigint");

                    b.Property<int>("Version")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchemaId");

                    b.ToTable("SchemaVersions");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Stage", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Stages");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Value", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<bool?>("BoolValue")
                        .HasColumnType("bit");

                    b.Property<double?>("DecimalValue")
                        .HasColumnType("float");

                    b.Property<long?>("IntegerValue")
                        .HasColumnType("bigint");

                    b.Property<string>("RawValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("RecordId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchemaFieldVersionId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.HasIndex("SchemaFieldVersionId");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.WebHook", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Events")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Version")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("WebHooks");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.WebHookEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<long>("Id"));

                    b.Property<int>("Attempts")
                        .HasColumnType("int");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<DateTime>("FirstAttempt")
                        .HasColumnType("datetime2");

                    b.Property<string>("LastResponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("MostRecentAttempt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("NextAttempt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PostData")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<bool>("Success")
                        .HasColumnType("bit");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("WebHookId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("WebHookId");

                    b.ToTable("WebHookEvents");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<System.Guid>", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.EctoRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<System.Guid>", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.EctoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<System.Guid>", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.EctoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<System.Guid>", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.EctoRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trogsoft.Ectobi.Data.EctoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<System.Guid>", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.EctoUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Batch", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Schema", null)
                        .WithMany("Batches")
                        .HasForeignKey("SchemaId");

                    b.HasOne("Trogsoft.Ectobi.Data.SchemaVersion", "SchemaVersion")
                        .WithMany()
                        .HasForeignKey("SchemaVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchemaVersion");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.LookupSetValue", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.LookupSet", "LookupSet")
                        .WithMany("Values")
                        .HasForeignKey("LookupSetId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LookupSet");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Period", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.SchemaVersion", "SchemaVersion")
                        .WithMany()
                        .HasForeignKey("SchemaVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SchemaVersion");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.ProcessElement", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Process", "Process")
                        .WithMany("Elements")
                        .HasForeignKey("ProcessId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Process");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Record", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Batch", "Batch")
                        .WithMany("Records")
                        .HasForeignKey("BatchId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Batch");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaField", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Schema", "Schema")
                        .WithMany("SchemaFields")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schema");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaFieldVersion", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.LookupSet", "LookupSet")
                        .WithMany()
                        .HasForeignKey("LookupSetId");

                    b.HasOne("Trogsoft.Ectobi.Data.Populator", "Populator")
                        .WithMany()
                        .HasForeignKey("PopulatorId");

                    b.HasOne("Trogsoft.Ectobi.Data.Process", "Process")
                        .WithMany()
                        .HasForeignKey("ProcessId");

                    b.HasOne("Trogsoft.Ectobi.Data.SchemaField", "SchemaField")
                        .WithMany()
                        .HasForeignKey("SchemaFieldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Trogsoft.Ectobi.Data.SchemaVersion", "SchemaVersion")
                        .WithMany("Fields")
                        .HasForeignKey("SchemaVersionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LookupSet");

                    b.Navigation("Populator");

                    b.Navigation("Process");

                    b.Navigation("SchemaField");

                    b.Navigation("SchemaVersion");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaVersion", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Schema", "Schema")
                        .WithMany("Versions")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schema");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Value", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Record", "Record")
                        .WithMany("Values")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trogsoft.Ectobi.Data.SchemaFieldVersion", "SchemaFieldVersion")
                        .WithMany()
                        .HasForeignKey("SchemaFieldVersionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Record");

                    b.Navigation("SchemaFieldVersion");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.WebHookEvent", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.WebHook", "WebHook")
                        .WithMany()
                        .HasForeignKey("WebHookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WebHook");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Batch", b =>
                {
                    b.Navigation("Records");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.LookupSet", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Process", b =>
                {
                    b.Navigation("Elements");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Record", b =>
                {
                    b.Navigation("Values");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Schema", b =>
                {
                    b.Navigation("Batches");

                    b.Navigation("SchemaFields");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.SchemaVersion", b =>
                {
                    b.Navigation("Fields");
                });
#pragma warning restore 612, 618
        }
    }
}
