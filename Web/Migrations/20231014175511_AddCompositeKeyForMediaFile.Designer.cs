﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Web.Data;

#nullable disable

namespace Web.Migrations
{
    [DbContext(typeof(CustomDbContext))]
    [Migration("20231014175511_AddCompositeKeyForMediaFile")]
    partial class AddCompositeKeyForMediaFile
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.11");

            modelBuilder.Entity("Web.Models.Account", b =>
                {
                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.Property<string>("AuthToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Username");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Web.Models.BusyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Completed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double?>("Progress")
                        .HasColumnType("REAL");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Web.Models.DownloadElement", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<long>("DownloadedBytes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ElementType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("Finished")
                        .HasColumnType("TEXT");

                    b.Property<bool>("FinishedSuccessfully")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MediaKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Progress")
                        .HasColumnType("REAL");

                    b.Property<DateTimeOffset?>("Started")
                        .HasColumnType("TEXT");

                    b.Property<long>("TotalBytes")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Downloads");
                });

            modelBuilder.Entity("Web.Models.Episode", b =>
                {
                    b.Property<string>("RatingKey")
                        .HasColumnType("TEXT");

                    b.Property<int>("EpisodeNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LibraryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SeasonNumber")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TvShowId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("RatingKey");

                    b.HasIndex("TvShowId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("Web.Models.Library", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Libraries");
                });

            modelBuilder.Entity("Web.Models.MediaFile", b =>
                {
                    b.Property<string>("DownloadUri")
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .HasColumnType("TEXT");

                    b.Property<double?>("AspectRatio")
                        .HasColumnType("REAL");

                    b.Property<int?>("AudioChannels")
                        .HasColumnType("INTEGER");

                    b.Property<string>("AudioCodec")
                        .HasColumnType("TEXT");

                    b.Property<string>("AudioProfile")
                        .HasColumnType("TEXT");

                    b.Property<long?>("Bitrate")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Container")
                        .HasColumnType("TEXT");

                    b.Property<long?>("Duration")
                        .HasColumnType("INTEGER");

                    b.Property<string>("EpisodeRatingKey")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LibraryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("MovieRatingKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("RatingKey")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerFilePath")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<long>("TotalBytes")
                        .HasColumnType("INTEGER");

                    b.Property<string>("VideoCodec")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoFrameRate")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoProfile")
                        .HasColumnType("TEXT");

                    b.Property<string>("VideoResolution")
                        .HasColumnType("TEXT");

                    b.Property<int?>("Width")
                        .HasColumnType("INTEGER");

                    b.HasKey("DownloadUri", "ServerId");

                    b.HasIndex("EpisodeRatingKey");

                    b.HasIndex("MovieRatingKey");

                    b.ToTable("MediaFiles");
                });

            modelBuilder.Entity("Web.Models.Movie", b =>
                {
                    b.Property<string>("RatingKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LibraryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("RatingKey");

                    b.ToTable("Movies");
                });

            modelBuilder.Entity("Web.Models.Playlist", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Items")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ServerId");

                    b.ToTable("Playlists");
                });

            modelBuilder.Entity("Web.Models.Server", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessToken")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsOnline")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastKnownUri")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("LastModified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("OwnerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SourceTitle")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Servers");
                });

            modelBuilder.Entity("Web.Models.ServerConnection", b =>
                {
                    b.Property<string>("Uri")
                        .HasColumnType("TEXT");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IpV6")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Local")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Port")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Protocol")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("Relay")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Uri");

                    b.HasIndex("ServerId");

                    b.ToTable("ServerConnection");
                });

            modelBuilder.Entity("Web.Models.Setting", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastModified")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("Settings");
                });

            modelBuilder.Entity("Web.Models.TvShow", b =>
                {
                    b.Property<string>("RatingKey")
                        .HasColumnType("TEXT");

                    b.Property<string>("Guid")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("LibraryId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ServerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("RatingKey");

                    b.ToTable("TvShows");
                });

            modelBuilder.Entity("Web.Models.Episode", b =>
                {
                    b.HasOne("Web.Models.TvShow", "TvShow")
                        .WithMany("Episodes")
                        .HasForeignKey("TvShowId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TvShow");
                });

            modelBuilder.Entity("Web.Models.Library", b =>
                {
                    b.HasOne("Web.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("Web.Models.MediaFile", b =>
                {
                    b.HasOne("Web.Models.Episode", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("EpisodeRatingKey");

                    b.HasOne("Web.Models.Movie", null)
                        .WithMany("MediaFiles")
                        .HasForeignKey("MovieRatingKey");
                });

            modelBuilder.Entity("Web.Models.Playlist", b =>
                {
                    b.HasOne("Web.Models.Server", "Server")
                        .WithMany()
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("Web.Models.ServerConnection", b =>
                {
                    b.HasOne("Web.Models.Server", "Server")
                        .WithMany("Connections")
                        .HasForeignKey("ServerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Server");
                });

            modelBuilder.Entity("Web.Models.Episode", b =>
                {
                    b.Navigation("MediaFiles");
                });

            modelBuilder.Entity("Web.Models.Movie", b =>
                {
                    b.Navigation("MediaFiles");
                });

            modelBuilder.Entity("Web.Models.Server", b =>
                {
                    b.Navigation("Connections");
                });

            modelBuilder.Entity("Web.Models.TvShow", b =>
                {
                    b.Navigation("Episodes");
                });
#pragma warning restore 612, 618
        }
    }
}
