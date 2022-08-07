using Microsoft.EntityFrameworkCore;
using Web.Models;

namespace Web.Data;

public class SettingContext : DbContext
{
    public DbSet<PlexAccount> PlexAccounts { get; set; }

    public SettingContext(DbContextOptions<SettingContext> options)
        : base(options)
    {
    }
}