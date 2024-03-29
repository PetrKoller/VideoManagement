﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using VideoManagement.Database;

#nullable disable

namespace PowerTrainer.VideoManagement.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.14")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PowerTrainer.VideoManagement.Features.Videos.Entity.Video", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_on_utc");

                    b.Property<string>("DestinationLocation")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("destination_location");

                    b.Property<string>("DownloadFileLocation")
                        .HasColumnType("text")
                        .HasColumnName("download_file_location");

                    b.Property<DateTime?>("EncodedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("encoded_on_utc");

                    b.Property<string>("EncodingJobId")
                        .HasColumnType("text")
                        .HasColumnName("encoding_job_id");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("text")
                        .HasColumnName("error_message");

                    b.Property<Guid>("ExternalId")
                        .HasColumnType("uuid")
                        .HasColumnName("external_id");

                    b.Property<bool>("IsDownloadable")
                        .HasColumnType("boolean")
                        .HasColumnName("is_downloadable");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("name");

                    b.Property<string>("OriginalFileLocation")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("original_file_location");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uuid")
                        .HasColumnName("owner_id");

                    b.Property<string>("OwnerName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)")
                        .HasColumnName("owner_name");

                    b.Property<int>("Status")
                        .HasColumnType("integer")
                        .HasColumnName("status");

                    b.Property<string>("StreamFileLocation")
                        .HasColumnType("text")
                        .HasColumnName("stream_file_location");

                    b.Property<DateTime?>("UploadedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("uploaded_on_utc");

                    b.HasKey("Id")
                        .HasName("pk_videos");

                    b.HasIndex("EncodingJobId")
                        .HasDatabaseName("ix_videos_encoding_job_id");

                    b.HasIndex("ExternalId")
                        .IsUnique()
                        .HasDatabaseName("ix_videos_external_id");

                    b.ToTable("videos", (string)null);
                });

            modelBuilder.Entity("PowerTrainer.VideoManagement.Outbox.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id");

                    b.Property<string>("Data")
                        .IsRequired()
                        .HasColumnType("json")
                        .HasColumnName("data");

                    b.Property<string>("Error")
                        .HasColumnType("text")
                        .HasColumnName("error");

                    b.Property<DateTime>("OccuredOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("occured_on_utc");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("processed_on_utc");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("type");

                    b.HasKey("Id")
                        .HasName("pk_outbox_messages");

                    b.ToTable("outbox_messages", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
