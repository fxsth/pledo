using Microsoft.EntityFrameworkCore;
using Web.Constants;
using Web.Data;
using Web.Models;
using Web.Models.DTO;
using DbContext = Web.Data.DbContext;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly DbContext _dbContext;

    public SettingsService(UnitOfWork unitOfWork, DbContext dbContext)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
    }

    public async Task<string> GetMovieDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.MovieDirectoryKey);
        if (setting == null)
            throw new InvalidOperationException("The movie directory setting is missing in db.");
        return setting.Value;
    }

    public async Task<string> GetEpisodeDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.EpisodeDirectoryKey);
        if (setting == null)
            throw new InvalidOperationException("The episode directory setting is missing in db.");
        return setting.Value;
    }

    public async Task<MovieFileTemplate> GetMovieFileTemplate()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.MovieFileTemplateKey);
        if (setting == null)
            throw new InvalidOperationException("The movie file template setting is missing in db.");
        if (Enum.TryParse(setting.Value, out MovieFileTemplate fileTemplate))
            return fileTemplate;
        return default;
    }

    public async Task<EpisodeFileTemplate> GetEpisodeFileTemplate()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.EpisodeFileTemplateKey);
        if (setting == null)
            throw new InvalidOperationException("The episode file template setting is missing in db.");
        if (Enum.TryParse(setting.Value, out EpisodeFileTemplate fileTemplate))
            return fileTemplate;
        return default;
    }

    public async Task<string?> GetPreferredResolution()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.PreferredResolutionKey);
        return setting?.Value;
    }

    public async Task<string?> GetPreferredVideoCodec()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(SettingsConstants.PreferredVideoCodec);
        return setting?.Value;
    }

    public Task<IEnumerable<SettingsResource>> GetSettings()
    {
        var settings = _unitOfWork.SettingRepository.GetAll();
        return Task.FromResult(settings.Select(x =>
        {
            var settingsResource = new SettingsResource()
            {
                Key = x.Key,
                Description = x.Description,
                Value = x.Value,
                Name = x.Name ?? "",
                Type = x.Type
            };
            AddOptions(settingsResource);
            return settingsResource;
        }));
    }

    private void AddOptions(SettingsResource settingsResource)
    {
        if (settingsResource.Key == SettingsConstants.EpisodeFileTemplateKey)
            settingsResource.Options = new[]
            {
                new Option(EpisodeFileTemplate.SeriesDirectoryAndFilenameFromServer.ToString(),
                    "<Download directory>/<Tv Show>/<Episode.ext>"),
                new Option(EpisodeFileTemplate.SeriesAndSeasonDirectoriesAndFilenameFromServer.ToString(),
                    "<Download directory>/<Tv Show>/<Season>/<Episode.ext>")
            };
        if (settingsResource.Key == SettingsConstants.MovieFileTemplateKey)
            settingsResource.Options = new[]
            {
                new Option(MovieFileTemplate.FilenameFromServer.ToString(), "<Download directory>/<Movie.ext>"),
                new Option(MovieFileTemplate.MovieDirectoryAndFilenameFromServer.ToString(),
                    "<Download directory>/<Movie>/<Movie.ext>")
            };
    }

    public Task ValidateSettings(IReadOnlyCollection<SettingsResource> settings)
    {
        ValidateDirectorySetting(settings.Single(x => x.Key == SettingsConstants.MovieDirectoryKey));
        ValidateDirectorySetting(settings.Single(x => x.Key == SettingsConstants.EpisodeDirectoryKey));
        return Task.CompletedTask;
    }

    private void ValidateDirectorySetting(SettingsResource setting)
    {
        try
        {
            Path.GetFullPath(setting.Value);
        }
        catch (Exception)
        {
            throw new ArgumentException($"{setting.Name} is not a valid directory.");
        }
    }

    public async Task UpdateSettings(IReadOnlyCollection<SettingsResource> settings)
    {
        foreach (var setting in settings)
        {
            var settingFromDb = await _unitOfWork.SettingRepository.GetById(setting.Key);
            if (settingFromDb == null)
                continue;

            settingFromDb.Value = setting.Value;
        }

        await _unitOfWork.Save();
    }

    public async Task<bool> ResetDatabase()
    {
        await _dbContext.Database.CloseConnectionAsync();
        bool reset = await _dbContext.Database.EnsureDeletedAsync() && await _dbContext.Database.EnsureCreatedAsync();
        DbInitializer.Initialize(_dbContext);
        return reset;
    }
}