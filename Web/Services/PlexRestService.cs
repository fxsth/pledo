using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Web.Exceptions;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;
using PlexAccount = Plex.ServerApi.PlexModels.Account.PlexAccount;

namespace Web.Services;

public class PlexRestService : IPlexRestService
{
    private readonly IPlexAccountClient _plexAccountClient;
    private readonly IPlexLibraryClient _plexLibraryClient;
    private readonly IPlexServerClient _plexServerClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<PlexRestService> _logger;
    private readonly PlexLibraryIterator _libraryIterator;

    public PlexRestService(IPlexAccountClient plexAccountClient,
        IPlexLibraryClient plexLibraryClient, IPlexServerClient plexServerClient, IHttpClientFactory httpClientFactory,
        ILogger<PlexRestService> logger, PlexLibraryIterator libraryIterator)
    {
        _plexAccountClient = plexAccountClient;
        _plexLibraryClient = plexLibraryClient;
        _plexServerClient = plexServerClient;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _libraryIterator = libraryIterator;
    }

    public async Task<PlexAccount?> LoginAccount(CredentialsResource credentialsResource)
    {
        return await _plexAccountClient.GetPlexAccountAsync(credentialsResource.username, credentialsResource.password);
    }
    
    public async Task<Account?> GetMyPlexAccount(string authToken)
    {
        var account = await _plexAccountClient.GetPlexAccountAsync(authToken);
        if (account == null)
            return null;
        return new Account()
        {
            Email = account.Email,
            Id = account.Id,
            Title = account.Title,
            Username = account.Username,
            Uuid = account.Uuid,
            AuthToken = account.AuthToken
        };
    }

    public async Task<IEnumerable<Server>> RetrieveServers(Account account)
    {
        var resources = await _plexAccountClient.GetResourcesAsync(account.AuthToken);
        var serverList = resources.Where(x => x.Provides == "server");
        return serverList.Select(x => new Server()
        {
            Id = x.ClientIdentifier,
            Name = x.Name,
            SourceTitle = x.SourceTitle,
            AccessToken = x.AccessToken,
            LastModified = DateTimeOffset.Now,
            OwnerId = x.OwnerId,
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
    
    public async Task<string?> GetTransientToken(Server server)
    {
        try
        {
            string? uri = server.LastKnownUri;
            if (string.IsNullOrEmpty(uri))
                uri = server.Connections.First().ToString();
            var transientTokenContainer = await _plexServerClient.GetTransientToken(server.AccessToken, uri);
            var token = transientTokenContainer.Token;
            return token;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while retrieving transient token from server {0}", server.Name);
            return null;
        }
    }

    public async Task<IEnumerable<Library>> RetrieveLibraries(Server server)
    {
        try
        {
            string? uri = server.LastKnownUri;
            if (string.IsNullOrEmpty(uri))
                uri = server.Connections.First().ToString();
            LibraryContainer libraryContainer =
                await _plexServerClient.GetLibrariesAsync(server.AccessToken, uri);
            server.LastKnownUri = uri;
            return libraryContainer.Libraries.Select(x => new Library()
            {
                Id = x.Uuid,
                Key = x.Key,
                Name = x.Title, Type = x.Type,
                ServerId = server.Id
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured while retrieving libraries from server {0}", server.Name);
            return Enumerable.Empty<Library>();
        }
    }

    public async Task<Movie?> RetrieveMovieByKey(Library library, string movieKey)
    {
        var mediaContainer =
            await _plexLibraryClient.GetItem(library.Server.AccessToken, library.Server.LastKnownUri, movieKey);
        if (mediaContainer.Media == null)
            return null;
        IEnumerable<Movie> movies = Mapper.GetMoviesFromMediaContainer(mediaContainer, library, _logger).ToList();
        if (movies.Count() > 1)
            throw new InvalidDataException();
        return movies.First();
    }

    public async Task<IEnumerable<Movie>> RetrieveMovies(Library library)
    {
        return await _libraryIterator.GetWithDynamicBatchSize<Movie>(library, 50, TimeSpan.FromMilliseconds(200));
    }

    public async Task<IEnumerable<Episode>> RetrieveEpisodes(Library library)
    {
        return await _libraryIterator.GetWithDynamicBatchSize<Episode>(library, 50, TimeSpan.FromMilliseconds(200));
    }

    public async Task<IEnumerable<Playlist>> RetrievePlaylists(Server server)
    {
        var playlistContainer = await _plexServerClient.GetPlaylists(server.AccessToken, server.LastKnownUri);
        if (playlistContainer.Metadata == null)
            return Enumerable.Empty<Playlist>();
        List<Playlist> playlistList = new List<Playlist>();
        foreach (var playlistMetadata in playlistContainer.Metadata)
        {
            var playlistItems =
                await _plexServerClient.GetPlaylistItems(server.AccessToken, server.LastKnownUri, playlistMetadata);
            if (playlistItems?.Media == null)
            {
                _logger.LogWarning("Syncing playlists: Playlist {0} does not contain any media.",
                    playlistMetadata.Title);
                continue;
            }

            playlistList.Add(new Playlist()
            {
                Id = playlistMetadata.RatingKey,
                Name = playlistMetadata.Title,
                ServerId = server.Id,
                Items = playlistItems.Media.Select(x => x.RatingKey).ToList()
            });
        }

        return playlistList;
    }

    public async Task<IEnumerable<TvShow>> RetrieveTvShows(Library library)
    {
        List<TvShow> tvShows = new List<TvShow>();
        int offset = 0;
        int limit = 24;
        while (true)
        {
            var retrieveTvShows = (await RetrieveTvShows(library, offset, limit)).ToList();
            if (retrieveTvShows.Any())
                tvShows.AddRange(retrieveTvShows);
            else
                break;
            offset += limit;
        }

        return tvShows;
    }

    private async Task<IEnumerable<TvShow>> RetrieveTvShows(Library library, int offset, int limit)
    {
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, SearchType.Show, null, offset, limit);
        if (mediaContainer.Media == null)
            return Enumerable.Empty<TvShow>();
        IEnumerable<TvShow> episodes = mediaContainer.Media.Select(x => new TvShow()
        {
            Title = x.Title,
            Key = x.Key,
            RatingKey = x.RatingKey,
            Guid = x.Guid,
            LibraryId = library.Id,
            ServerId = library.ServerId
        });
        return episodes;
    }

    public IReadOnlyCollection<Uri> GetAllPossibleConnectionUrisForServer(Server server)
    {
        var resourceConnections = server.Connections;
        if (resourceConnections.Any() != true)
            throw new ArgumentException("No resource connections specified.");
        List<Uri> uris = resourceConnections.Where(x => !x.Relay).Select(x=>new Uri(x.Uri)).ToList();
        uris.AddRange(resourceConnections.Where(x => !x.Relay).Select(x=>
        {
            return new UriBuilder("http", x.Address) { Port = x.Port }.Uri;
        }).ToList());
        uris.AddRange(resourceConnections.Where(x => x.Relay).Select(x=>new Uri(x.Uri)).ToList());
        return uris;
    }

    public async Task<string> GetUriFromFastestConnection(Server server, int timeoutInSeconds)
    {
        IReadOnlyCollection<Uri> urisToTest = GetAllPossibleConnectionUrisForServer(server);
        List<Task> tasks = new List<Task>();
        string? uri = null;
        try
        {
            CancellationTokenSource cancellationTokenSource =
                new CancellationTokenSource(TimeSpan.FromSeconds(timeoutInSeconds));
            HttpClient httpClient = _httpClientFactory.CreateClient();
            foreach (Uri connectionForSync in urisToTest)
            {
                try
                {
                    Uri serverInfoUri = new UriBuilder(connectionForSync)
                        { Query = $"?X-Plex-Token={server.AccessToken}" }.Uri;
                    tasks.Add(httpClient.GetAsync(serverInfoUri, cancellationTokenSource.Token).ContinueWith(t =>
                    {
                        _logger.LogTrace("{2} ---- {0} {1}", t.Result.StatusCode, t.Result.ReasonPhrase,
                            serverInfoUri);
                        if (t.IsCompletedSuccessfully && t.Result.IsSuccessStatusCode)
                        {
                            if (string.IsNullOrEmpty(uri))
                            {
                                uri = t.Result.RequestMessage?.RequestUri?.ToString().Split('?')[0];
                            }

                            if (!string.IsNullOrEmpty(uri))
                                cancellationTokenSource.Cancel();
                        }
                    }, cancellationTokenSource.Token));
                }
                catch (Exception e)
                {
                    _logger.LogTrace(e, "Connecting to {0} threw an exception:", connectionForSync.AbsolutePath);
                }
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception e)
            {
                _logger.LogTrace(e, "Waiting for responses from all server connections, an error occured.");
                //ignored
            }

            if (string.IsNullOrEmpty(uri))
                throw new ServerUnreachableException(server.Name);
        }
        catch (ServerUnreachableException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "An unexpected error occured while trying to check server connections of {0}.", server.Name);
        }

        return uri;
    }
}