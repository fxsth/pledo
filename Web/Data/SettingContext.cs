using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class SettingContext : DbContext
{
    public DbSet<Account> PlexAccounts { get; set; }
    public DbSet<Server> Servers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<BusyTask> Tasks { get; set; }

    public SettingContext(DbContextOptions<SettingContext> options)
        : base(options)
    {
    }
}