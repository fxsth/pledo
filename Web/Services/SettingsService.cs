using Microsoft.EntityFrameworkCore;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Library.Search;
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
        var serversFromDb = _context.Servers;
        if (serversFromDb?.Any() == true)
            return serversFromDb;
        var account = _context.PlexAccounts.FirstOrDefault();
        if (account != null)
        {
            var servers = (await RetrieveServers(account)).ToList();
            await _context.Servers.AddRangeAsync(servers);
            await _context.SaveChangesAsync();

            return servers;
        }
        else
            return Enumerable.Empty<Server>();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverName)
    {
        var librariesFromDb = _context.Libraries.Include(x => x.Server)?.Where(x => x.Server.Name == serverName);
        if (librariesFromDb?.Any() == true)
            return librariesFromDb;
        var server = _context.Servers.Include(x => x.Connections).FirstOrDefault(x => x.Name == serverName);
        if (server != null)
        {
            string uri = await GetUriFromFastestConnection(server.Connections);
            _context.Servers.Attach(server);
            server.LastKnownUri = uri;
            await _context.SaveChangesAsync();
            var libraries = await RetrieveLibraries(server);
            IEnumerable<Library> librariesList = libraries.ToList();
            var newLibraries = librariesList.Except(_context.Libraries);
            await _context.AddRangeAsync(newLibraries);
            await _context.SaveChangesAsync();
            return librariesList;
        }
        else
            return Enumerable.Empty<Library>();
    }

    public async Task<IEnumerable<Movie>> GetMovies(string libraryId)
    {
        var library = _context.Libraries.Include(x => x.Server).FirstOrDefault(x => x.Id == libraryId);
        if (library != null)
        {
            var movies = await RetrieveMovies(library);
            return movies;
        }
        else
            return Enumerable.Empty<Movie>();
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

    private async Task<IEnumerable<Server>> RetrieveServers(Account account)
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

    private async Task<IEnumerable<Library>> RetrieveLibraries(Server server)
    {
        LibraryContainer libraryContainer =
            await _plexServerClient.GetLibrariesAsync(server.AccessToken, server.LastKnownUri);
        return libraryContainer.Libraries.Select(x => new Library()
        {
            Id = x.Uuid,
            Key = x.Key,
            Name = x.Title, Type = x.Type,
            Server = server
        });
    }

    private async Task<IEnumerable<Movie>> RetrieveMovies(Library library)
    {
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, SearchType.Movie);
        IEnumerable<Movie> movies = mediaContainer.Media.Where(x => x.Media?.First()?.Part?.First()?.File != null)
            .Select(x => new Movie()
            {
                Title = x.Title,
                Key = x.Key,
                RatingKey = x.RatingKey,
                ServerFilePath = x.Media.First().Part.First().File,
                DownloadUri = x.Media.First().Part.First().Key
            });
        return movies;
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