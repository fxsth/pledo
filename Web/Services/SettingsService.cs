using Microsoft.EntityFrameworkCore;
using Web.Data;
using Web.Models;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly SettingContext _context;

    public SettingsService(SettingContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<PlexAccount>> GetPlexAccounts()
    {
        return _context.PlexAccounts;
    }
    
    public async Task AddPlexAccount(PlexAccount plexAccount)
    {
        await _context.PlexAccounts.AddAsync(plexAccount);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemovePlexAccount(string username)
    {
        PlexAccount account = new PlexAccount() {Username = username};
        _context.PlexAccounts.Attach(account);
        _context.PlexAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }
}