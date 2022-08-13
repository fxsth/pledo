using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.PlexModels.Account.Resources;
using Web.Data;
using Web.Models;
using PlexAccount = Plex.ServerApi.PlexModels.Account.PlexAccount;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly SettingContext _context;
    private readonly IPlexAccountClient _plexAccountClient;

    public SettingsService(SettingContext context, IPlexAccountClient plexAccountClient)
    {
        _context = context;
        _plexAccountClient = plexAccountClient;
    }

    public async Task<IEnumerable<Account>> GetPlexAccounts()
    {
        return _context.PlexAccounts;
    }

    public async Task<IEnumerable<Server>> GetServers()
    {
        var account = _context.PlexAccounts.FirstOrDefault();
        if (account != null)
            return await GetServers(account);
        else
            return Enumerable.Empty<Server>();
    }

    public async Task<IEnumerable<Library>> GetLibraries()
    {
        return _context.Libraries;
    }

    public async Task<IEnumerable<BusyTask>> GetTasks()
    {
        return _context.Tasks;
    }

    public async Task<bool> AddPlexAccount(Account account)
    {
        var plexAccount = await LoginAccount(account);
        if (plexAccount == null)
            return false;
        account.UserToken = plexAccount.AuthToken;
        account.Title = plexAccount.Title;
        await _context.PlexAccounts.AddAsync(account);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task RemovePlexAccount(string username)
    {
        Account account = new Account() { Username = username };
        _context.PlexAccounts.Attach(account);
        _context.PlexAccounts.Remove(account);
        await _context.SaveChangesAsync();
    }

    private async Task<PlexAccount?> LoginAccount(Account plexAccount)
    {
        return await _plexAccountClient.GetPlexAccountAsync(plexAccount.Username, plexAccount.Password);
    }

    private async Task<IEnumerable<Server>> GetServers(Account account)
    {
        var resources = await _plexAccountClient.GetResourcesAsync(account.UserToken);
        var serverList = resources.Where(x => x.Provides == "server");
        return serverList.Select(x => new Server()
        {
            Id = x.Name,
            Name = x.Name, 
            Connections = x.Connections.Select(y => new ServerConnection()
            {
                Uri = y.Uri,
                Address = y.Address,
                Local = y.Local,
                Port = y.Port,
                Protocol = y.Protocol,
                Relay = y.Relay,
                IpV6 = y.IpV6
            }).ToList()
        });
    }
}