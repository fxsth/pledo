using Web.Data;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly IPlexService _plexService;
    private readonly ILibraryRepository _libraryRepository;
    private readonly IServerRepository _serverRepository;
    private readonly IMovieRepository _movieRepository;
    private readonly IAccountRepository _accountRepository;

    public SettingsService(IPlexService plexService,
        ILibraryRepository libraryRepository,
        IServerRepository serverRepository, IMovieRepository movieRepository,
        IAccountRepository accountRepository)
    {
        _plexService = plexService;
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
            var servers = (await _plexService.RetrieveServers(account)).ToList();
            await _serverRepository.Upsert(servers);

            return servers;
        }
        else
            return Enumerable.Empty<Server>();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverId)
    {
        var server = (await _serverRepository.GetAll()).FirstOrDefault(x => x.Name == serverId);
        if (server != null)
        {
            string uri = await _plexService.GetUriFromFastestConnection(server);
            server.LastKnownUri = uri;
            await _serverRepository.Update(new[] { server });
            // var libraries = (await RetrieveLibraries(server)).ToList();
            // await _libraryRepository.Upsert(libraries);
            var libraries = (await _libraryRepository.GetAll()).Where(x => x.Server.Id == serverId);
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
            // var movies = (await RetrieveMovies(library)).ToList();
            // await _movieRepository.Upsert(movies);
            var movies = (await _movieRepository.GetAll()).Where(x => x.LibraryId == libraryId);
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
        var plexAccount = await _plexService.LoginAccount(credentials);
        if (plexAccount == null)
            return false;
        Account account = new Account()
        {
            Title = plexAccount.Title,
            Username = plexAccount.Username,
            UserToken = plexAccount.AuthToken
        };
        await _accountRepository.Insert(new[] { account });
        return true;
    }

    public async Task RemovePlexAccount(string username)
    {
        Account account = new Account() { Username = username };
        await _accountRepository.Remove(account);
    }

}