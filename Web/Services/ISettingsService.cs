using Web.Models;

namespace Web.Services;

public interface ISettingsService
{
    Task<IEnumerable<Account>> GetPlexAccounts();
    Task<IEnumerable<Server>> GetServers();
    Task<IEnumerable<Library>> GetLibraries();
    Task<IEnumerable<BusyTask>> GetTasks();

    Task<bool> AddPlexAccount(Account account);

    Task RemovePlexAccount(string username);
}