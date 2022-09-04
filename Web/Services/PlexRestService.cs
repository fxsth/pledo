using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
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

    public PlexRestService(IPlexAccountClient plexAccountClient,
        IPlexLibraryClient plexLibraryClient, IPlexServerClient plexServerClient)
    {
        _plexAccountClient = plexAccountClient;
        _plexLibraryClient = plexLibraryClient;
        _plexServerClient = plexServerClient;
    }

    public async Task<PlexAccount?> LoginAccount(Credentials credentials)
    {
        return await _plexAccountClient.GetPlexAccountAsync(credentials.username, credentials.password);
    }

    public async Task<IEnumerable<Server>> RetrieveServers(Account account)
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

    public async Task<IEnumerable<Library>> RetrieveLibraries(Server server)
    {
        LibraryContainer libraryContainer =
            await _plexServerClient.GetLibrariesAsync(server.AccessToken, server.LastKnownUri);
        return libraryContainer.Libraries.Select(x => new Library()
        {
            Id = x.Uuid,
            Key = x.Key,
            Name = x.Title, Type = x.Type,
            ServerId = server.Id
        });
    }

    public async Task<Movie> RetrieveMovieByKey(Library library, string movieKey)
    {
        var mediaContainer =
            await _plexLibraryClient.GetItem(library.Server.AccessToken, library.Server.LastKnownUri, movieKey);
        if (mediaContainer.Media == null)
            return null;
        IEnumerable<Movie> movies = mediaContainer.Media
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
        if (movies.Count() > 1)
            throw new InvalidDataException();
        return movies.First();
    }

    public async Task<IEnumerable<Movie>> RetrieveMovies(Library library)
    {
        List<Movie> movies = new List<Movie>();
        int offset = 0;
        int limit = 200;
        while (true)
        {
            var retrieveMovies = (await RetrieveMovies(library, offset, limit)).ToList();
            if (retrieveMovies.Any())
                movies.AddRange(retrieveMovies);
            else
                break;
            offset += limit;
        }

        return movies;
    }

    public async Task<IEnumerable<Episode>> RetrieveEpisodes(Library library)
    {
        List<Episode> tvShows = new List<Episode>();
        int offset = 0;
        int limit = 24;
        while (true)
        {
            var retrieveTvShows = (await RetrieveEpisodes(library, offset, limit)).ToList();
            if (retrieveTvShows.Any())
                tvShows.AddRange(retrieveTvShows);
            else
                break;
            offset += limit;
        }

        return tvShows;
    }

    private async Task<IEnumerable<Episode>> RetrieveEpisodes(Library library, int offset, int limit)
    {
        
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, SearchType.Episode, null, offset, limit);
        if (mediaContainer.Media == null)
            return Enumerable.Empty<Episode>();
        IEnumerable<Episode> episodes = mediaContainer.Media.Where(x => x.Media?.First()?.Part?.First()?.File != null)
            .Select(x => new Episode()
            {
                Title = x.Title,
                Key = x.Key,
                RatingKey = x.RatingKey,
                LibraryId = library.Id,
                ServerId = library.ServerId,
                DownloadUri = x.Media.First().Part.First().Key,
                TotalBytes = x.Media.First().Part.First().Size,
                ServerFilePath = x.Media.First().Part.First().File,
                EpisodeNumber = x.Index,
                SeasonNumber = x.ParentIndex,
                TvShowId = x.GrandparentRatingKey
            });
        return episodes;
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

    public async Task<string> GetUriFromFastestConnection(Server server)
    {
        var resourceConnections = server.Connections;
        if (resourceConnections?.Any() != true)
            throw new ArgumentException("No resource connections specified.");
        List<Task> tasks = new List<Task>();
        string uri = "";
        try
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            HttpClient httpClient = new HttpClient();
            List<ServerConnection> connections = resourceConnections.Where(x => !x.Relay).ToList();
            connections.AddRange(resourceConnections.Where(x => x.Relay).ToList());
            foreach (ServerConnection connectionForSync in connections)
            {
                try
                {
                    Uri serverInfoUri = new UriBuilder(connectionForSync.Uri)
                        { Query = $"?X-Plex-Token={server.AccessToken}" }.Uri;
                    tasks.Add(httpClient.GetAsync(serverInfoUri, cancellationTokenSource.Token).ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully && t.Result.IsSuccessStatusCode)
                        {
                            if (string.IsNullOrEmpty(uri))
                            {
                                uri = t.Result.RequestMessage.RequestUri?.ToString().Split('?')[0];
                            }

                            cancellationTokenSource.Cancel();
                        }
                    }, cancellationTokenSource.Token));
                }
                catch (Exception)
                {
                }
            }

            await Task.WhenAll(tasks);
            if (string.IsNullOrEmpty(uri))
                throw new InvalidOperationException("Could not get uris for connecting to server.");
        }
        catch
        {
            // ignored
        }

        return uri;
    }
}