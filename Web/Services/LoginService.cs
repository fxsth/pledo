using Plex.ServerApi;
using Plex.ServerApi.Clients.Interfaces;
using Plex.ServerApi.PlexModels.OAuth;
using Web.Data;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public class LoginService : ILoginService
{
    private readonly IPlexRestService _plexRestService;
    private readonly IPlexAccountClient _plexAccountClient;
    private readonly UnitOfWork _unitOfWork;
    private static OAuthPin? _oAuthPin;

    public LoginService(IPlexRestService plexRestService, IPlexAccountClient plexAccountClient, ClientOptions clientOptions,
        UnitOfWork unitOfWork)
    {
        _plexRestService = plexRestService;
        _plexAccountClient = plexAccountClient;
        _unitOfWork = unitOfWork;
    }

    private bool IsLoginPending()
    {
        return _oAuthPin != null;
    }

    public async Task<Account?> GetPlexAccount()
    {
        if (IsLoginPending())
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

    public async Task<bool> AddPlexAccount(CredentialsResource credentialsResource)
    {
        var plexAccount = await _plexRestService.LoginAccount(credentialsResource);
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
}