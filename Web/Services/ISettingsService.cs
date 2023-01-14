using System.Runtime.CompilerServices;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<string> GetMovieDirectory();
    Task<string> GetEpisodeDirectory();
    Task<IEnumerable<SettingsResource>> GetSettings();
    Task UpdateSettings(IEnumerable<SettingsResource> settings);
    Task<bool> ResetDatabase();
}