using Microsoft.EntityFrameworkCore.ChangeTracking;
using Web.Data;
using Web.Models;

namespace Web.Services;

public class SyncService : ISyncService
{
    private IPlexService _plexService;
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
                _plexService = scope.ServiceProvider.GetRequiredService<IPlexService>();
                var serverRepository = scope.ServiceProvider.GetRequiredService<IServerRepository>();
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
                var libraryRepository = scope.ServiceProvider.GetRequiredService<ILibraryRepository>();
                var movieRepository = scope.ServiceProvider.GetRequiredService<IMovieRepository>();
                await SyncServers(accountRepository, serverRepository);
                await SyncConnections(serverRepository);
                await SyncLibraries(libraryRepository, serverRepository);
                await SyncMovies(libraryRepository, movieRepository);
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

    private async Task SyncServers(IAccountRepository accountRepository, IServerRepository serverRepository)
    {
        _syncTask = new BusyTask() { Name = "Sync servers" };
        var account = (await accountRepository.GetAll()).FirstOrDefault();
        if (account != null)
        {
            var serversInDb = await serverRepository.GetAll();
            var newServers = (await _plexService.RetrieveServers(account)).ToList();
            var toRemove = serversInDb.ExceptBy(newServers.Select(x => x.Id), server => server.Id);
            foreach (var server in toRemove)
            {
                await serverRepository.Remove(server);
            }

            await serverRepository.Upsert(newServers);
        }
    }

    private async Task SyncConnections(IServerRepository serverRepository)
    {
        _syncTask = new BusyTask() { Name = "Sync server connections" };
        IEnumerable<Server> servers = await serverRepository.GetAll();
        foreach (var server in servers)
        {
            var uriFromFastestConnection = await _plexService.GetUriFromFastestConnection(server);
            server.LastKnownUri = uriFromFastestConnection;
            await serverRepository.Upsert(new[] { server });
        }
    }

    private async Task SyncLibraries(ILibraryRepository libraryRepository, IServerRepository serverRepository)
    {
        _syncTask = new BusyTask() { Name = "Sync libraries" };
        IEnumerable<Server> servers = await serverRepository.GetAll();
        var librariesInDb = (await libraryRepository.GetAll()).ToList();
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
    }

    private async Task SyncMovies(ILibraryRepository libraryRepository, IMovieRepository movieRepository)
    {
        _syncTask = new BusyTask() { Name = "Sync movies" };
        IEnumerable<Library> libraries = await libraryRepository.GetAll();
        foreach (var library in libraries)
        {
            var movies = (await _plexService.RetrieveMovies(library)).ToList();
            await movieRepository.Upsert(movies);
        }
    }
}