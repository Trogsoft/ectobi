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
    [Migration("20230321160905_RemoveRecordTextId")]
    partial class RemoveRecordTextId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

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

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("SchemaId")
                        .HasColumnType("bigint");

                    b.Property<string>("Source")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("SchemaId");

                    b.ToTable("Batches");
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

                    b.Property<int>("Flags")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long?>("PopulatorId")
                        .HasColumnType("bigint");

                    b.Property<long?>("ProcessId")
                        .HasColumnType("bigint");

                    b.Property<long>("SchemaId")
                        .HasColumnType("bigint");

                    b.Property<string>("TextId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<long?>("ValuesFromSchemaId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("PopulatorId");

                    b.HasIndex("ProcessId");

                    b.HasIndex("SchemaId");

                    b.HasIndex("ValuesFromSchemaId");

                    b.ToTable("SchemaFields");
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

                    b.Property<long>("SchemaFieldId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("RecordId");

                    b.HasIndex("SchemaFieldId");

                    b.ToTable("Values");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Batch", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Schema", "Schema")
                        .WithMany("Batches")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schema");
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
                    b.HasOne("Trogsoft.Ectobi.Data.Populator", "Populator")
                        .WithMany()
                        .HasForeignKey("PopulatorId");

                    b.HasOne("Trogsoft.Ectobi.Data.Process", "Process")
                        .WithMany()
                        .HasForeignKey("ProcessId");

                    b.HasOne("Trogsoft.Ectobi.Data.Schema", "Schema")
                        .WithMany("SchemaFields")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trogsoft.Ectobi.Data.Schema", "ValuesFromSchema")
                        .WithMany()
                        .HasForeignKey("ValuesFromSchemaId");

                    b.Navigation("Populator");

                    b.Navigation("Process");

                    b.Navigation("Schema");

                    b.Navigation("ValuesFromSchema");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Value", b =>
                {
                    b.HasOne("Trogsoft.Ectobi.Data.Record", "Record")
                        .WithMany("Values")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Trogsoft.Ectobi.Data.SchemaField", "SchemaField")
                        .WithMany()
                        .HasForeignKey("SchemaFieldId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Record");

                    b.Navigation("SchemaField");
                });

            modelBuilder.Entity("Trogsoft.Ectobi.Data.Batch", b =>
                {
                    b.Navigation("Records");
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
                });
#pragma warning restore 612, 618
        }
    }
}
