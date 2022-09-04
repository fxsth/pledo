using Plex.ServerApi;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.PlexModels.OAuth;
using Web.Data;
using Web.Models;
using Web.Models.DTO;
using Library = Web.Models.Library;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly IPlexRestService _plexRestService;
    private readonly IPlexAccountClient _plexAccountClient;
    private readonly ClientOptions _clientOptions;
    private readonly UnitOfWork _unitOfWork;
    private static OAuthPin? _oAuthPin;

    public SettingsService(IPlexRestService plexRestService, IPlexAccountClient plexAccountClient, ClientOptions clientOptions,
        UnitOfWork unitOfWork)
    {
        _plexRestService = plexRestService;
        _plexAccountClient = plexAccountClient;
        _clientOptions = clientOptions;
        _unitOfWork = unitOfWork;
    }

    public async Task<Account?> GetPlexAccount()
    {
        if (_oAuthPin != null)
        {
            OAuthPin? authTokenFromOAuthPinAsync = await _plexAccountClient.GetAuthTokenFromOAuthPinAsync(_oAuthPin.Id.ToString());
            var account = await _plexAccountClient.GetPlexAccountAsync(authTokenFromOAuthPinAsync.AuthToken);
            var plexAccount = new Account()
            {
                Email = account.Email,
                Id = account.Id,
                Title = account.Title,
                Username = account.Username,
                Uuid = account.Uuid,
                AuthToken = account.AuthToken
            };
            await _unitOfWork.AccountRepository.Insert(plexAccount);
            _oAuthPin = null;
            await _unitOfWork.Save();
            return plexAccount;
        }
        return _unitOfWork.AccountRepository.GetAll().FirstOrDefault();
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
        var plexAccount = await _plexRestService.LoginAccount(credentials);
        if (plexAccount == null)
            return false;
        Account account = new Account()
        {
            Title = plexAccount.Title,
            Username = plexAccount.Username,
            AuthToken = plexAccount.AuthToken
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

    public async Task<string> GeneratePlexAuthUrl(Uri forwardUri)
    {
        var oAuthPinAsync = await _plexAccountClient.CreateOAuthPinAsync(Uri.EscapeDataString(forwardUri.ToString()));
        _oAuthPin = oAuthPinAsync;
        var uri = oAuthPinAsync.Url.Replace("[", "%5B");
        uri = uri.Replace("]", "%5D");
        return uri;
    }
    
    private Uri GetPlexAuthAppUrl(string clientId, string code, string forwardUrl, string appName)
    {
        string uri;
        if (forwardUrl == null)
            uri = $"https://app.plex.tv/auth#!?clientID={clientId}&code={code}&context%5Bdevice%5D%5Bproduct%5D={appName}";
        else
            uri = $"https://app.plex.tv/auth#!?clientID={clientId}&code={code}&context%5Bdevice%5D%5Bproduct%5D={appName}&forwardUrl={forwardUrl}";
        return new Uri(uri);
    }
}