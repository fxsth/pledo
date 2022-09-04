using Web.Data;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly IPlexRestService _plexService;
    private readonly UnitOfWork _unitOfWork;

    public SettingsService(IPlexRestService plexService,
        UnitOfWork unitOfWork)
    {
        _plexService = plexService;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Account>> GetPlexAccounts()
    {
        return  _unitOfWork.AccountRepository.GetAll();
    }

    public async Task<IEnumerable<Server>> GetServers()
    {
        return _unitOfWork.ServerRepository.GetAll();
    }

    public async Task<IEnumerable<Library>> GetLibraries(string serverId)
    {
        return _unitOfWork.LibraryRepository.Get(x => x.ServerId == serverId);
    }

    public async Task<IEnumerable<Movie>> GetMovies(string libraryId)
    {
        return _unitOfWork.MovieRepository.Get(x => x.LibraryId == libraryId);
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
        await _unitOfWork.Save();
        return true;
    }

    public async Task RemovePlexAccount(string username)
    {
        Account account = new Account() { Username = username };
        await _unitOfWork.AccountRepository.Remove(account);
    }
}