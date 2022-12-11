using Web.Data;
using Web.Models;

namespace Web.Services;

public class SyncService : ISyncService
{
    private IPlexRestService _plexService = null!;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncService> _logger;

    private BusyTask? _syncTask;

    public SyncService(IServiceScopeFactory scopeFactory, ILogger<SyncService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public BusyTask? GetCurrentSyncTask()
    {
        return _syncTask;
    }

    public async Task SyncAll()
    {
        try
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                _plexService = scope.ServiceProvider.GetRequiredService<IPlexRestService>();
                UnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<UnitOfWork>();
                IReadOnlyCollection<Server> syncServers = await SyncServers(unitOfWork);
                IReadOnlyCollection<Server> servers = await SyncConnections(syncServers, unitOfWork);
                IReadOnlyCollection<Library> libraries = await SyncLibraries(servers, unitOfWork);
                await SyncMovies(libraries, unitOfWork);
                await SyncTvShows(libraries, unitOfWork);
                await SyncEpisodes(libraries, unitOfWork);
                await unitOfWork.Save();
            }
        }
        catch(Exception e)
        {
            _logger.Log(LogLevel.Error, e, "An unexpected error occured while syncing:");
        }
        finally
        {
            _syncTask = null;
        }
    }

    private async Task<IReadOnlyCollection<Server>> SyncServers(UnitOfWork unitOfWork)
    {
        AccountRepository accountRepository = unitOfWork.AccountRepository;
        ServerRepository serverRepository = unitOfWork.ServerRepository;
        _syncTask = new BusyTask() { Name = "Sync servers" };
        var account = accountRepository.GetAll().FirstOrDefault();
        if (account != null)
        {
            var serversInDb = serverRepository.GetAll();
            var newServers = (await _plexService.RetrieveServers(account)).ToList();
            var toRemove = serversInDb.ExceptBy(newServers.Select(x => x.Id), server => server.Id);
            _logger.LogInformation("Syncing servers: {0} new ({1})", newServers.Count,
                string.Join(", ", newServers.Select(x => x.Name)));
            foreach (var server in toRemove)
            {
                _logger.LogWarning("Removing unlisted server {0} from database.", server.Name);
                await serverRepository.Remove(server);
            }

            // await serverRepository.Upsert(newServers);
            return newServers;
        }

        return new List<Server>();
    }

    private async Task<IReadOnlyCollection<Server>> SyncConnections(IReadOnlyCollection<Server> servers,
        UnitOfWork unitOfWork)
    {
        _syncTask = new BusyTask() { Name = "Sync server connections" };
        foreach (var server in servers)
        {
            var uriFromFastestConnection = await _plexService.GetUriFromFastestConnection(server);
            _logger.LogInformation("Found fastest connection uri {0} for server {1}", uriFromFastestConnection,
                server.Name);
            server.LastKnownUri = uriFromFastestConnection;
            server.LastModified = DateTimeOffset.Now;
        }

        ServerRepository serverRepository = unitOfWork.ServerRepository;
        await serverRepository.Upsert(servers);
        return servers;
    }

    private async Task<IReadOnlyCollection<Library>> SyncLibraries(IEnumerable<Server> servers, UnitOfWork unitOfWork)
    {
        var libraryRepository = unitOfWork.LibraryRepository;
        _syncTask = new BusyTask() { Name = "Sync libraries" };
        var librariesInDb = libraryRepository.GetAll();
        List<Library> librariesFromApi = new List<Library>();
        IEnumerable<Server> serverList = servers.ToList();
        foreach (var server in serverList)
        {
            var libraries = (await _plexService.RetrieveLibraries(server)).ToList();
            _logger.LogInformation("Syncing libraries: Found {0} ({1}) of server {2}", libraries.Count,
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

    private async Task SyncMovies(IEnumerable<Library> libraries, UnitOfWork unitOfWork)
    {
        var movieRepository = unitOfWork.MovieRepository;
        _syncTask = new BusyTask() { Name = "Sync movies" };
        libraries = libraries.Where(x => x.Type == "movie");
        List<Movie> movies = new List<Movie>();
        foreach (var library in libraries)
        {
            var moviesFromThisLibrary = (await _plexService.RetrieveMovies(library)).ToList();
            _logger.LogInformation("Syncing movies: Found {0} movies in library {1} from server {2}",
                moviesFromThisLibrary.Count, library.Name, library.ServerId);
            movies.AddRange(moviesFromThisLibrary);
        }

        await movieRepository.Upsert(movies);
    }

    private async Task SyncTvShows(IEnumerable<Library> libraries, UnitOfWork unitOfWork)
    {
        var tvShowRepository = unitOfWork.TvShowRepository;
        _syncTask = new BusyTask() { Name = "Sync TV shows" };
        libraries = libraries.Where(x => x.Type == "show");
        List<TvShow> tvShows = new List<TvShow>();
        foreach (var library in libraries)
        {
            var tvShowsFromLibrary = (await _plexService.RetrieveTvShows(library)).ToList();
            _logger.LogInformation("Syncing tv shows: Found {0} tv shows in library {1} from server {2}",
                tvShowsFromLibrary.Count, library.Name, library.ServerId);
            tvShows.AddRange(tvShowsFromLibrary);
        }

        await tvShowRepository.Upsert(tvShows.DistinctBy(x => x.RatingKey));
    }

    private async Task SyncEpisodes(IEnumerable<Library> libraries, UnitOfWork unitOfWork)
    {
        var episodeRepository = unitOfWork.EpisodeRepository;
        _syncTask = new BusyTask() { Name = "Sync episodes" };
        libraries = libraries.Where(x => x.Type == "show");
        List<Episode> episodes = new List<Episode>();
        foreach (var library in libraries)
        {
            var episodesFromThisLibrary = (await _plexService.RetrieveEpisodes(library)).ToList();
            _logger.LogInformation("Syncing episodes: Found {0} episodes in library {1} from server {2}",
                episodesFromThisLibrary.Count, library.Name, library.ServerId);
            episodes.AddRange(episodesFromThisLibrary);
        }

        await episodeRepository.Upsert(episodes.DistinctBy(x => x.RatingKey));
    }
}