using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<IEnumerable<Account>> GetPlexAccounts();
    Task<IEnumerable<Server>> GetServers();
    Task<IEnumerable<Library>> GetLibraries(string serverName);
    Task<IEnumerable<BusyTask>> GetTasks();
    Task<IEnumerable<Movie>> GetMovies(string libraryId);

    Task<bool> AddPlexAccount(Credentials credentials);

    Task RemovePlexAccount(string username);
}