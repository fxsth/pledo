﻿using Polly;
using Web.Data;
using Web.Exceptions;
using Web.Models;

namespace Web.Services;

public class SyncService : ISyncService
{
    private IPlexRestService _plexService = null!;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncService> _logger;

    private BusyTask? _syncTask;
    private readonly AsyncPolicy _policy;

    public SyncService(IServiceScopeFactory scopeFactory, ILogger<SyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _policy = Policy
            .Handle<ServerUnreachableException>()
            .RetryAsync(2,
                onRetry: (_, retryCount, ctx) =>
                {
                    _logger.LogWarning("Server not reachable, retry connections with longer timeout for a {0}. time.",
                        retryCount);
                    ctx["TryCount"] = retryCount;
                });
    }

    public BusyTask? GetCurrentSyncTask()
    {
        return _syncTask;
    }

    public async Task Sync(SyncType syncType)
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _logger.LogInformation("{0} sync started.", syncType.ToString());
                
                _plexService = scope.ServiceProvider.GetRequiredService<IPlexRestService>();
                UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                Account? account = await SyncAccount(unitOfWork);
                if (string.IsNullOrEmpty(account?.AuthToken))
                { 
                    _logger.LogInformation("Cannot sync available media server due to missing authorization token. A login for plex.tv is necessary.");
                    return;
                }
                IReadOnlyCollection<Server> syncServers = await SyncServers(account, unitOfWork);
                IReadOnlyCollection<Server> servers = await SyncConnections(syncServers, unitOfWork);
                IReadOnlyCollection<Server> onlineServers = servers.Where(x => x.IsOnline).ToList();
                if (syncType == SyncType.Full)
                {
                    IReadOnlyCollection<Library> libraries = await SyncLibraries(onlineServers, unitOfWork);
                    var movies = await SyncMovies(libraries);
                    await SyncTvShows(libraries, unitOfWork);
                    var episodes = await SyncEpisodes(libraries);
                    await UpsertMediaIntoDb(unitOfWork, movies, episodes);
                    await SyncPlaylists(onlineServers, unitOfWork);
                }
                await unitOfWork.Save();
                
                _logger.LogInformation("{0} sync completed.", syncType.ToString());
            }
        }
        catch (Exception e)
        {
            _logger.Log(LogLevel.Error, e, "An unexpected error occured while syncing:");
        }
        finally
        {
            _syncTask = null;
        }
    }

    private async Task<Account?> SyncAccount(UnitOfWork unitOfWork)
    {
        IReadOnlyCollection<Account> accounts = unitOfWork.AccountRepository.GetAll();
        if (!accounts.Any())
            return null;
        var account = accounts.First();
        Account? myPlexAccount = await _plexService.GetMyPlexAccount(account.AuthToken);
        if (myPlexAccount == null)
            return null;
        await unitOfWork.AccountRepository.Update(myPlexAccount);
        return myPlexAccount;
    }

    private async Task UpsertMediaIntoDb(UnitOfWork unitOfWork, IReadOnlyCollection<Movie> movies, IReadOnlyCollection<Episode> episodes)
    {
        var itemsFromApi = movies.SelectMany(x => x.MediaFiles).ToList();
        itemsFromApi.AddRange(episodes.SelectMany(x => x.MediaFiles));
        var itemsInDb = unitOfWork.MediaFileRepository.GetAll();
        var itemsToDelete = itemsInDb.Where(x=>!itemsFromApi.Contains(x));
        var itemsToInsert = itemsFromApi.Where(x=>!itemsInDb.Contains(x));
        var itemsToUpdate = itemsInDb.Where(x=>itemsFromApi.Contains(x));
        await unitOfWork.MediaFileRepository.Remove(itemsToDelete);
        await unitOfWork.MovieRepository.Upsert(movies);
        await unitOfWork.EpisodeRepository.Upsert(episodes);
        await unitOfWork.MediaFileRepository.Insert(itemsToInsert);
        await unitOfWork.MediaFileRepository.Update(itemsToUpdate);
    }

    private async Task SyncPlaylists(IReadOnlyCollection<Server> servers, UnitOfWork unitOfWork)
    {
        _syncTask = new BusyTask() { Name = "Syncing playlists" };
        List<Playlist> playlists = new List<Playlist>();
        foreach (var server in servers)
        {
            try
            {
                var playlistOfThisServer = await _plexService.RetrievePlaylists(server);
                var playlistList = playlistOfThisServer.ToList();
                _logger.LogInformation("Syncing {0} playlists from server {1}",
                    playlistList.Count(), server.Id);
                playlists.AddRange(playlistList);
            }
            catch (Exception)
            {
                _logger.LogError("Could not retrieve playlist metadata of server {0}.", server.Name);
            }
        }

        await unitOfWork.PlaylistRepository.Upsert(playlists);
    }

    private async Task<IReadOnlyCollection<Server>> SyncServers(Account? account, UnitOfWork unitOfWork)
    {
        ServerRepository serverRepository = unitOfWork.ServerRepository;
        _syncTask = new BusyTask() { Name = "Syncing servers" };
        if (string.IsNullOrEmpty(account?.AuthToken))
            throw new ArgumentException("Cannot sync available media server due to missing authorization token. A login for plex.tv is necessary.");
                
        var serversInDb = serverRepository.GetAll();
        var serversFromApi = (await _plexService.RetrieveServers(account)).ToList();
        var toRemove = serversInDb.ExceptBy(serversFromApi.Select(x => x.Id), server => server.Id);
        _logger.LogInformation("Syncing {0} servers: ({1})", serversFromApi.Count,
            string.Join(", ", serversFromApi.Select(x => x.Name)));
        await serverRepository.Remove(toRemove);
        // foreach (var server in toRemove)
        // {
        //     _logger.LogWarning("Removing unlisted server {0} from database.", server.Name);
        //     await serverRepository.Remove(server);
        // }

        // await serverRepository.Upsert(newServers);
        return serversFromApi;
    }

    private async Task<IReadOnlyCollection<Server>> SyncConnections(IReadOnlyCollection<Server> servers,
        UnitOfWork unitOfWork)
    {
        _syncTask = new BusyTask() { Name = "Syncing server connections" };
        foreach (var server in servers)
        {
            try
            {
                var uriFromFastestConnection = await _policy.ExecuteAsync<string?>(
                    async ctx =>
                    {
                        int count = (int)(ctx.Values.FirstOrDefault() ?? 0);
                        return await _plexService.GetUriFromFastestConnection(server, 5*3^count);
                    },
                    new Context());
                server.LastKnownUri = uriFromFastestConnection;
                server.LastModified = DateTimeOffset.Now;
                server.IsOnline = !string.IsNullOrEmpty(uriFromFastestConnection);

                if (server.IsOnline)
                {
                    _logger.LogInformation("Found fastest connection uri {0} for server {1}", uriFromFastestConnection,
                        server.Name);
                    var transientToken = await _plexService.GetTransientToken(server);
                    server.TransientToken = transientToken;
                }
                else
                    _logger.LogInformation("Server {0} seems to be offline, all connection attempts failed.", server.Name);
            }
            catch (ServerUnreachableException)
            {
                server.LastKnownUri = null;
                server.LastModified = DateTimeOffset.Now;
                server.IsOnline = false;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occured while syncing metadata.");
            }
        }

        ServerRepository serverRepository = unitOfWork.ServerRepository;
        await serverRepository.Upsert(servers);
        return servers;
    }

    private async Task<IReadOnlyCollection<Library>> SyncLibraries(IEnumerable<Server> servers, UnitOfWork unitOfWork)
    {
        var libraryRepository = unitOfWork.LibraryRepository;
        _syncTask = new BusyTask() { Name = "Syncing libraries" };
        var librariesInDb = libraryRepository.GetAll();
        List<Library> librariesFromApi = new List<Library>();
        IEnumerable<Server> serverList = servers.ToList();
        foreach (var server in serverList)
        {
            var libraries = (await _plexService.RetrieveLibraries(server)).ToList();
            _logger.LogInformation("Syncing {0} libraries: ({1}) of server {2}", libraries.Count,
                string.Join(", ", libraries.Select(x => x.Name)), server.Name);
            librariesFromApi.AddRange(libraries);
        }

        var toRemove = librariesInDb.ExceptBy(librariesFromApi.Select(x => x.Id), library => library.Id);
        var toAdd = librariesFromApi.ExceptBy(librariesInDb.Select(x => x.Id), library => library.Id);
        await libraryRepository.Insert(toAdd);
        await libraryRepository.Remove(toRemove);

        List<Library> librariesWithServers = new List<Library>();
        foreach (var library in librariesFromApi)
        {
            librariesWithServers.Add(new Library()
            {
                Id = library.Id,
                Key = library.Key,
                Name = library.Name,
                Type = library.Type,
                ServerId = library.ServerId,
                Server = serverList.First(x => x.Id == library.ServerId)
            });
        }

        return librariesWithServers;
    }

    private async Task<List<Movie>> SyncMovies(IEnumerable<Library> libraries)
    {
        _syncTask = new BusyTask() { Name = "Syncing movies" };
        libraries = libraries.Where(x => x.Type == "movie");
        List<Movie> movies = new List<Movie>();
        foreach (var library in libraries)
        {
            var moviesFromThisLibrary = (await _plexService.RetrieveMovies(library)).ToList();
            _logger.LogInformation("Syncing {0} movies in library {1} from server {2}",
                moviesFromThisLibrary.Count, library.Name, library.ServerId);
            movies.AddRange(moviesFromThisLibrary);
        }
        
        return movies;
    }

    private async Task SyncTvShows(IEnumerable<Library> libraries, UnitOfWork unitOfWork)
    {
        var tvShowRepository = unitOfWork.TvShowRepository;
        _syncTask = new BusyTask() { Name = "Syncing TV shows" };
        libraries = libraries.Where(x => x.Type == "show");
        List<TvShow> tvShows = new List<TvShow>();
        foreach (var library in libraries)
        {
            var tvShowsFromLibrary = (await _plexService.RetrieveTvShows(library)).ToList();
            _logger.LogInformation("Syncing {0} tv shows in library {1} from server {2}",
                tvShowsFromLibrary.Count, library.Name, library.ServerId);
            tvShows.AddRange(tvShowsFromLibrary);
        }

        await tvShowRepository.Upsert(tvShows.DistinctBy(x => x.RatingKey));
    }

    private async Task<List<Episode>> SyncEpisodes(IEnumerable<Library> libraries)
    {
        _syncTask = new BusyTask() { Name = "Syncing episodes" };
        libraries = libraries.Where(x => x.Type == "show");
        List<Episode> episodes = new List<Episode>();
        foreach (var library in libraries)
        {
            var episodesFromThisLibrary = (await _plexService.RetrieveEpisodes(library)).ToList();
            _logger.LogInformation("Syncing {0} episodes in library {1} from server {2}",
                episodesFromThisLibrary.Count, library.Name, library.ServerId);
            episodes.AddRange(episodesFromThisLibrary);
        }

        return episodes;
    }
}