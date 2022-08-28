using Web.Data;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly IPlexService _plexService;
    private readonly UnitOfWork _unitOfWork;

    public SettingsService(IPlexService plexService,
        UnitOfWork unitOfWork)
    {
        _plexService = plexService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Account>> GetPlexAccounts()
    {
        return await _unitOfWork.AccountRepository.GetAll();
    }

    public async Task<IEnumerable<Server>> GetServers()
    {
        return await _unitOfWork.ServerRepository.GetAll();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverId)
    {
        return (await _unitOfWork.LibraryRepository.GetAll()).Where(x => x.ServerId == serverId);
    }

    public async Task<IEnumerable<Movie>> GetMovies(string libraryId)
    {
        return (await _unitOfWork.MovieRepository.GetAll()).Where(x => x.LibraryId == libraryId);
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
        await _unitOfWork.AccountRepository.Insert(new[] { account });
        return true;
    }

    public async Task RemovePlexAccount(string username)
    {
        Account account = new Account() { Username = username };
        await _unitOfWork.AccountRepository.Remove(account);
    }
}