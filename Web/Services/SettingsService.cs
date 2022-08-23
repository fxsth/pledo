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
    private readonly IPlexAccountClient _plexAccountClient;
    private readonly IPlexLibraryClient _plexLibraryClient;
    private readonly IPlexServerClient _plexServerClient;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IServerRepository _serverRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IAccountRepository _accountRepository;

    public SettingsService(IPlexAccountClient plexAccountClient,
        IPlexLibraryClient plexLibraryClient, IPlexServerClient plexServerClient,
        ILibraryRepository libraryRepository,
        IServerRepository serverRepository, IMovieRepository movieRepository, 
        IAccountRepository accountRepository)
    {
        _plexAccountClient = plexAccountClient;
        _plexLibraryClient = plexLibraryClient;
        _plexServerClient = plexServerClient;
        _libraryRepository = libraryRepository;
        _serverRepository = serverRepository;
        _movieRepository = movieRepository;
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<Account>> GetPlexAccounts()
    {
        return await _accountRepository.GetAll();
    }

    public async Task<IEnumerable<Server>> GetServers()
    {
        var account = (await _accountRepository.GetAll()).FirstOrDefault();
        if (account != null)
        {
            var servers = (await RetrieveServers(account)).ToList();
            await _serverRepository.Upsert(servers);

            return servers;
        }
        else
            return Enumerable.Empty<Server>();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverId)
    {
        var server = (await _serverRepository.GetAll()).FirstOrDefault(x=>x.Name==serverId);
        if (server != null)
        {
            string uri = await GetUriFromFastestConnection(server.Connections);
            server.LastKnownUri = uri;
            await _serverRepository.Update(new []{server});
            var libraries = (await RetrieveLibraries(server)).ToList();
            await _libraryRepository.Upsert(libraries);
            return libraries;
        }
        else
            return Enumerable.Empty<Library>();
    }

    public async Task<IEnumerable<Movie>> GetMovies(string libraryId)
    {
        var library = await _libraryRepository.GetById(libraryId);
        if (library != null)
        {
            var movies = (await RetrieveMovies(library)).ToList();
            await _movieRepository.Upsert(movies);
            return movies;
        }
        else
            return Enumerable.Empty<Movie>();
    }

    public async Task<IEnumerable<BusyTask>> GetTasks()
    {
        // return _context.Tasks;
        throw new NotImplementedException();
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
        await _accountRepository.Insert(new []{account});
        return true;
    }

    public async Task RemovePlexAccount(string username)
    {
        Account account = new Account() { Username = username };
        await _accountRepository.Remove(new []{account});
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
        List<Movie> movies = new List<Movie>();
        int offset = 0;
        int limit = 100;
        while (true)
        {
            var retrieveMovies = (await RetrieveMovies(library, offset, offset + limit)).ToList();
            if (retrieveMovies.Any())
                movies.AddRange(retrieveMovies);
            else
                break;
            offset += limit;
        }

        return movies;
    }

    private async Task<IEnumerable<Movie>> RetrieveMovies(Library library, int offset, int limit)
    {
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, SearchType.Movie, null, offset, limit);
        if (mediaContainer.Media == null)
            return Enumerable.Empty<Movie>();
        IEnumerable<Movie> movies = mediaContainer.Media.Where(x => x.Media?.First()?.Part?.First()?.File != null)
            .Select(x => new Movie()
            {
                Title = x.Title,
                Key = x.Key,
                RatingKey = x.RatingKey,
                ServerFilePath = x.Media.First().Part.First().File,
                DownloadUri = x.Media.First().Part.First().Key,
                LibraryId = library.Id,
                ServerId = library.Server.Id,
                TotalBytes = x.Media.First().Part.First().Size
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