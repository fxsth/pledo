using Web.Data;
using Web.Models;

namespace Web.Services;

public class SyncService : ISyncService
{
    private IPlexRestService _plexService;
    private readonly IServiceScopeFactory _scopeFactory;

    private BusyTask? _syncTask;

    public SyncService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
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
                IEnumerable<Server> servers = await SyncConnections(syncServers, unitOfWork);
                IReadOnlyCollection<Library> libraries = await SyncLibraries(servers, unitOfWork);
                await SyncMovies(libraries, unitOfWork);
                await SyncTvShows(libraries, unitOfWork);
                await SyncEpisodes(libraries, unitOfWork);
                await unitOfWork.Save();
            }
        }
        catch
        {
            // ignored
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
            foreach (var server in toRemove)
            {
                await serverRepository.Remove(server);
            }

            // await serverRepository.Upsert(newServers);
            return newServers;
        }

        return new List<Server>();
    }

    private async Task<IEnumerable<Server>> SyncConnections(IEnumerable<Server> servers, UnitOfWork unitOfWork)
    {
        ServerRepository serverRepository = unitOfWork.ServerRepository;
        _syncTask = new BusyTask() { Name = "Sync server connections" };
        foreach (var server in servers)
        {
            var uriFromFastestConnection = await _plexService.GetUriFromFastestConnection(server);
            server.LastKnownUri = uriFromFastestConnection;
            server.LastModified = DateTimeOffset.Now;
        }

        await serverRepository.Upsert(servers);
        return servers;
    }

    private async Task<IReadOnlyCollection<Library>> SyncLibraries(IEnumerable<Server> servers, UnitOfWork unitOfWork)
    {
        var libraryRepository = unitOfWork.LibraryRepository;

        _syncTask = new BusyTask() { Name = "Sync libraries" };
        var librariesInDb = libraryRepository.Get().ToList();
        List<Library> librariesFromApi = new List<Library>();
        foreach (var server in servers)
        {
            var libraries = (await _plexService.RetrieveLibraries(server)).ToList();
            librariesFromApi.AddRange(libraries);
        }

        var toRemove = librariesInDb.ExceptBy(librariesFromApi.Select(x => x.Id), library => library.Id);
        var toAdd = librariesFromApi.ExceptBy(librariesInDb.Select(x => x.Id), library => library.Id);
        await libraryRepository.Insert(toAdd);
        foreach (var library in toRemove)
        {
            await libraryRepository.Remove(library);
        }

        return librariesFromApi;
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
            tvShows.AddRange(tvShowsFromLibrary);
        }

        await tvShowRepository.Upsert(tvShows.DistinctBy(x=>x.RatingKey));
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
            episodes.AddRange(episodesFromThisLibrary);
        }

        await episodeRepository.Upsert(episodes.DistinctBy(x=>x.RatingKey));
    }
}