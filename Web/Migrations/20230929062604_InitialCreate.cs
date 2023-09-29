using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    AuthToken = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    Uuid = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Downloads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Uri = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FinishedSuccessfully = table.Column<bool>(type: "INTEGER", nullable: false),
                    ElementType = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<double>(type: "REAL", nullable: false),
                    TotalBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    DownloadedBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Started = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    Finished = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    MediaKey = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Downloads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    RatingKey = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.RatingKey);
                });

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SourceTitle = table.Column<string>(type: "TEXT", nullable: true),
                    OwnerId = table.Column<int>(type: "INTEGER", nullable: true),
                    LastKnownUri = table.Column<string>(type: "TEXT", nullable: true),
                    IsOnline = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: true),
                    LastModified = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    IsHidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    Progress = table.Column<double>(type: "REAL", nullable: true),
                    Completed = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TvShows",
                columns: table => new
                {
                    RatingKey = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Guid = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShows", x => x.RatingKey);
                });

            migrationBuilder.CreateTable(
                name: "Libraries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Libraries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Libraries_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Items = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServerConnection",
                columns: table => new
                {
                    Uri = table.Column<string>(type: "TEXT", nullable: false),
                    Protocol = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Local = table.Column<bool>(type: "INTEGER", nullable: false),
                    Relay = table.Column<bool>(type: "INTEGER", nullable: false),
                    IpV6 = table.Column<bool>(type: "INTEGER", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerConnection", x => x.Uri);
                    table.ForeignKey(
                        name: "FK_ServerConnection_Servers_ServerId",
                        column: x => x.ServerId,
                        principalTable: "Servers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    RatingKey = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: true),
                    SeasonNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TvShowId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.RatingKey);
                    table.ForeignKey(
                        name: "FK_Episodes_TvShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TvShows",
                        principalColumn: "RatingKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    DownloadUri = table.Column<string>(type: "TEXT", nullable: false),
                    RatingKey = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    ServerFilePath = table.Column<string>(type: "TEXT", nullable: false),
                    MovieRatingKey = table.Column<string>(type: "TEXT", nullable: true),
                    EpisodeRatingKey = table.Column<string>(type: "TEXT", nullable: true),
                    TotalBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    LibraryId = table.Column<string>(type: "TEXT", nullable: false),
                    ServerId = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<long>(type: "INTEGER", nullable: true),
                    Bitrate = table.Column<long>(type: "INTEGER", nullable: true),
                    Width = table.Column<int>(type: "INTEGER", nullable: true),
                    Height = table.Column<int>(type: "INTEGER", nullable: true),
                    AspectRatio = table.Column<double>(type: "REAL", nullable: true),
                    AudioChannels = table.Column<int>(type: "INTEGER", nullable: true),
                    AudioCodec = table.Column<string>(type: "TEXT", nullable: true),
                    VideoCodec = table.Column<string>(type: "TEXT", nullable: true),
                    VideoResolution = table.Column<string>(type: "TEXT", nullable: true),
                    Container = table.Column<string>(type: "TEXT", nullable: true),
                    VideoFrameRate = table.Column<string>(type: "TEXT", nullable: true),
                    AudioProfile = table.Column<string>(type: "TEXT", nullable: true),
                    VideoProfile = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.DownloadUri);
                    table.ForeignKey(
                        name: "FK_MediaFiles_Episodes_EpisodeRatingKey",
                        column: x => x.EpisodeRatingKey,
                        principalTable: "Episodes",
                        principalColumn: "RatingKey");
                    table.ForeignKey(
                        name: "FK_MediaFiles_Movies_MovieRatingKey",
                        column: x => x.MovieRatingKey,
                        principalTable: "Movies",
                        principalColumn: "RatingKey");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_TvShowId",
                table: "Episodes",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_Libraries_ServerId",
                table: "Libraries",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_EpisodeRatingKey",
                table: "MediaFiles",
                column: "EpisodeRatingKey");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_MovieRatingKey",
                table: "MediaFiles",
                column: "MovieRatingKey");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ServerId",
                table: "Playlists",
                column: "ServerId");

            migrationBuilder.CreateIndex(
                name: "IX_ServerConnection_ServerId",
                table: "ServerConnection",
                column: "ServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Downloads");

            migrationBuilder.DropTable(
                name: "Libraries");

            migrationBuilder.DropTable(
                name: "MediaFiles");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "ServerConnection");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "TvShows");
        }
    }
}
