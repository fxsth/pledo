using Web.Models;

namespace Web.Services;

public interface ISettingsService
{
    Task<IEnumerable<PlexAccount>> GetPlexAccounts();

    Task AddPlexAccount(PlexAccount plexAccount);

    Task RemovePlexAccount(string username);
}