using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<Account?> GetPlexAccount();
    Task<IEnumerable<Server>> GetServers();
    Task<IEnumerable<Library>> GetLibraries(string serverName);
    Task<IEnumerable<Movie>> GetMovies(string libraryId);

    Task<bool> AddPlexAccount(Credentials credentials);

    Task RemovePlexAccount(string username);
    Task<string> GeneratePlexAuthUrl(Uri forwardUri);
}