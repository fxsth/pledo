using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ILoginService
{
    Task<Account?> GetPlexAccount();
    Task<bool> AddPlexAccount(CredentialsResource credentialsResource);
    Task RemovePlexAccount(string username);
    Task<string> GeneratePlexAuthUrl(Uri forwardUri);
}