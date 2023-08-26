using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public interface ISettingsService
{
    Task<string> GetMovieDirectory();
    Task<string> GetEpisodeDirectory();
    Task<MovieFileTemplate> GetMovieFileTemplate();
    Task<EpisodeFileTemplate> GetEpisodeFileTemplate();
    Task<IEnumerable<SettingsResource>> GetSettings();
    Task ValidateSettings(IReadOnlyCollection<SettingsResource> settings);
    Task UpdateSettings(IReadOnlyCollection<SettingsResource> settings);
    Task<bool> ResetDatabase();
}