using Microsoft.EntityFrameworkCore;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Web.Data;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;
using PlexAccount = Plex.ServerApi.PlexModels.Account.PlexAccount;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly SettingContext _context;
    private readonly IPlexAccountClient _plexAccountClient;
    private readonly IPlexLibraryClient _plexLibraryClient;
    private readonly IPlexServerClient _plexServerClient;

    public SettingsService(SettingContext context, IPlexAccountClient plexAccountClient,
        IPlexLibraryClient plexLibraryClient, IPlexServerClient plexServerClient)
    {
        _context = context;
        _plexAccountClient = plexAccountClient;
        _plexLibraryClient = plexLibraryClient;
        _plexServerClient = plexServerClient;
    }

    public async Task<IEnumerable<Account>> GetPlexAccounts()
    {
        return _context.PlexAccounts;
    }

    public async Task<IEnumerable<Server>> GetServers()
    {
        var account = _context.PlexAccounts.FirstOrDefault();
        if (account != null)
        {
            var servers = (await GetServers(account)).ToList();
            foreach (var server in servers)
            {
                if (_context.Servers.Any(x => x.Id == server.Id))
                    _context.Update(server);
                else
                    _context.Add(server);
            }

            _context.SaveChanges();

            return servers;
        }
        else
            return Enumerable.Empty<Server>();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverName)
    {
        var server = _context.Servers.Include(x=>x.Connections).FirstOrDefault(x => x.Name == serverName);
        if (server != null)
            return await GetLibraries(server);
        else
            return Enumerable.Empty<Library>();
    }

    public async Task<IEnumerable<BusyTask>> GetTasks()
    {
        return _context.Tasks;
    }

    public async Task<bool> AddPlexAccount(Credentials credentials)
    {
        var plexAccount = await LoginAccount(credentials);
        if (plexAccount == null)
            return false;
        Account account = new Account()
        {
            Title = plexAccount.Title,
            Username = plexAccount.Username,
            UserToken = plexAccount.AuthToken
        };
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

    private async Task<PlexAccount?> LoginAccount(Credentials credentials)
    {
        return await _plexAccountClient.GetPlexAccountAsync(credentials.username, credentials.password);
    }

    private async Task<IEnumerable<Server>> GetServers(Account account)
    {
        var resources = await _plexAccountClient.GetResourcesAsync(account.UserToken);
        var serverList = resources.Where(x => x.Provides == "server");
        return serverList.Select(x => new Server()
        {
            Id = x.Name,
            Name = x.Name,
            AccessToken = x.AccessToken,
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

    private async Task<IEnumerable<Library>> GetLibraries(Server server)
    {
        string uri = await GetUriFromFastestConnection(server.Connections);
        LibraryContainer libraryContainer = await _plexServerClient.GetLibrariesAsync(server.AccessToken, uri);
        return libraryContainer.Libraries.Select(x => new Library()
        {
            Id = x.Uuid,
            Key = x.Key,
            Name = x.Title, Type = x.Type
        });
    }

    private async Task<string> GetUriFromFastestConnection(IEnumerable<ServerConnection> resourceConnections)
    {
        if (resourceConnections?.Any() != true)
            throw new ArgumentException("No resource connections specified.");
        List<Task> tasks = new List<Task>();
        string uri = "";
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        HttpClient httpClient = new HttpClient();
        List<ServerConnection> connections = resourceConnections.Where(x => !x.Relay).ToList();
        connections.AddRange(resourceConnections.Where(x => x.Relay).ToList());
        foreach (ServerConnection connectionForSync in connections)
        {
            try
            {
                tasks.Add(httpClient.GetAsync(connectionForSync.Uri, cancellationTokenSource.Token).ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        uri = t.Result.RequestMessage.RequestUri?.ToString();
                        cancellationTokenSource.Cancel();
                    }
                }));
            }
            catch (Exception)
            {
            }
        }

        await Task.WhenAll(tasks);
        if (string.IsNullOrEmpty(uri))
            throw new InvalidOperationException("Could not get uris for connecting to server.");
        return uri;
    }
}