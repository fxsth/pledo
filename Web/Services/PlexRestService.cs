using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.Enums;
using Plex.ServerApi.PlexModels.Library;
using Plex.ServerApi.PlexModels.Media;
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
    private readonly ILogger<PlexRestService> _logger;

    public PlexRestService(IPlexAccountClient plexAccountClient,
        IPlexLibraryClient plexLibraryClient, IPlexServerClient plexServerClient, ILogger<PlexRestService> logger)
    {
        _plexAccountClient = plexAccountClient;
        _plexLibraryClient = plexLibraryClient;
        _plexServerClient = plexServerClient;
        _logger = logger;
    }

    public async Task<PlexAccount?> LoginAccount(CredentialsResource credentialsResource)
    {
        return await _plexAccountClient.GetPlexAccountAsync(credentialsResource.username, credentialsResource.password);
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

    public async Task<IEnumerable<Library>> RetrieveLibraries(Server server)
    {
        try
        {
            string uri = server.LastKnownUri ?? await GetUriFromFastestConnection(server);
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
        IEnumerable<Movie> movies = GetMovies(mediaContainer, library, _logger).ToList();
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
        int limit = 72;
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
            if(playlistItems?.Media == null)
                continue;
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

    private async Task<IEnumerable<Episode>> RetrieveEpisodes(Library library, int offset, int limit)
    {
        var mediaContainer = await _plexLibraryClient.LibrarySearch(library.Server.AccessToken,
            library.Server.LastKnownUri, null, library.Key,
            null, SearchType.Episode, null, offset, limit);
        if (mediaContainer.Media == null)
            return Enumerable.Empty<Episode>();
        IEnumerable<Episode> episodes = GetEpisodes(mediaContainer, library, _logger);
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
        IEnumerable<Movie> movies = GetMovies(mediaContainer, library, _logger);
        return movies;
    }

    public async Task<string> GetUriFromFastestConnection(Server server)
    {
        var resourceConnections = server.Connections;
        if (resourceConnections?.Any() != true)
            throw new ArgumentException("No resource connections specified.");
        List<Task> tasks = new List<Task>();
        string? uri = null;
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
                                uri = t.Result.RequestMessage?.RequestUri?.ToString().Split('?')[0];
                            }

                            if(!string.IsNullOrEmpty(uri))
                                cancellationTokenSource.Cancel();
                        }
                    }, cancellationTokenSource.Token));
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            await Task.WhenAll(tasks);

            if (string.IsNullOrEmpty(uri))
                throw new InvalidOperationException(
                    $"Could not get fastest uri for connecting to server {server.Name}.");
        }
        catch
        {
            // ignored
        }

        return uri;
    }

    private IEnumerable<Movie> GetMovies(MediaContainer mediaContainer, Library library, ILogger<PlexRestService> logger)
    {
        foreach (var x in mediaContainer.Media)
        {
            if (x.Media?.FirstOrDefault()?.Part?.Any() != true)
            {
                logger.LogWarning("Movie {0} will be skipped, because it does not contain any file.", x.Title);
                break;
            }
            if (x.Media.Count > 1 || x.Media.First().Part.Count > 1)
            {
                logger.LogTrace("Movie {0} contains more than one file.", x.Title);
            }

            List<MediaFile> mediaFiles = new List<MediaFile>();
            foreach (var medium in x.Media)
            {
                foreach (var part in medium.Part) 
                {
                    mediaFiles.Add(Map(medium, part, library, x));
                }
            }
            
            yield return new Movie()
            {
                Title = x.Title,
                Key = x.Key,
                RatingKey = x.RatingKey,
                LibraryId = library.Id,
                ServerId = library.Server.Id,
                Year = x.Year,
                MediaFiles = mediaFiles
            };
        }
    }
    
    private IEnumerable<Episode> GetEpisodes(MediaContainer mediaContainer, Library library, ILogger<PlexRestService> logger)
    {
        foreach (var x in mediaContainer.Media)
        {
            if (x.Media?.FirstOrDefault()?.Part?.Any() != true)
            {
                logger.LogWarning("Episode {0} will be skipped, because file is missing.", x.Title);
                break;
            }
            if (x.Media.Count > 1 || x.Media.First().Part.Count > 1)
            {
                logger.LogTrace("Episode {0} contains more than one file, this program does not support more than one file", x.Title);
            }

            List<MediaFile> mediaFiles = new List<MediaFile>();
            foreach (var medium in x.Media)
            {
                foreach (var part in medium.Part) 
                {
                    mediaFiles.Add(Map(medium, part, library, x));
                }
            }
            
            yield return new Episode()
            {
                Title = x.Title,
                Key = x.Key,
                RatingKey = x.RatingKey,
                LibraryId = library.Id,
                ServerId = library.ServerId,
                Year = x.Year,
                MediaFiles = mediaFiles,
                EpisodeNumber = x.Index,
                SeasonNumber = x.ParentIndex,
                TvShowId = x.GrandparentRatingKey,
            };
        }
    }

    private static MediaFile Map(Medium medium, MediaPart part, Library library, Metadata x)
    {
        return new MediaFile()
        {
            Key = x.Key,
            RatingKey = x.RatingKey,
            ServerFilePath = part.File,
            DownloadUri = part.Key,
            LibraryId = library.Id,
            ServerId = library.Server.Id,
            TotalBytes = part.Size,
            Bitrate = medium.Bitrate,
            Container = medium.Container,
            Duration = medium.Duration,
            Height = medium.Height,
            Width = medium.Width,
            AspectRatio = medium.AspectRatio,
            AudioChannels = medium.AudioChannels,
            AudioCodec = medium.AudioCodec,
            AudioProfile = medium.AudioProfile,
            VideoCodec = medium.VideoCodec,
            VideoProfile = medium.VideoProfile,
            VideoResolution = medium.VideoResolution,
            VideoFrameRate = medium.VideoFrameRate
        };
    }
}