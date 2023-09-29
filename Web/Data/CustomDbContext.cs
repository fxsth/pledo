using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class CustomDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<TvShow> TvShows { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<BusyTask> Tasks { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<DownloadElement> Downloads { get; set; }
    public DbSet<Playlist> Playlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Playlist>()
            .Property(b => b.Items)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
        
        modelBuilder.Entity<Movie>()
            .HasMany(e => e.MediaFiles)
            .WithOne()
            .HasForeignKey(e => e.MovieRatingKey);
        
        modelBuilder.Entity<Episode>()
            .HasMany(e => e.MediaFiles)
            .WithOne()
            .HasForeignKey(e => e.EpisodeRatingKey);
    }

    public CustomDbContext(DbContextOptions<CustomDbContext> options)
        : base(options)
    {
    }
}