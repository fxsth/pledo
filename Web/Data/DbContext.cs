using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class DbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<TvShow> TvShows { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<BusyTask> Tasks { get; set; }
    public DbSet<KeyValueSetting> Settings { get; set; }
    public DbSet<DownloadElement> Downloads { get; set; }
    public DbSet<Playlist> Playlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Playlist>()
            .Property(b => b.Items)
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null));
    }

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }
}