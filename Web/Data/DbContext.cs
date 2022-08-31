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

    public DbContext(DbContextOptions<DbContext> options)
        : base(options)
    {
    }
}