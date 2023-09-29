using Microsoft.EntityFrameworkCore;
using Web.Constants;
using Web.Data;
using Web.Models;
using Web.Models.DTO;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private readonly UnitOfWork _unitOfWork;
    private readonly CustomDbContext _customDbContext;

    public SettingsService(UnitOfWork unitOfWork, CustomDbContext customDbContext)
    {
        _unitOfWork = unitOfWork;
        _customDbContext = customDbContext;
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
        if (settingsResource.Key == SettingsConstants.PreferredResolutionKey)
            settingsResource.Options = new[]
            {
                new Option("", "No preference"),
                new Option("sd", "SD"),
                new Option("480", "480p"),
                new Option("576", "576p"),
                new Option("720", "720p"),
                new Option("1080", "1080p"),
                new Option("2k", "2k"),
                new Option("4k", "4k"),
            };
        if (settingsResource.Key == SettingsConstants.PreferredVideoCodec)
            settingsResource.Options = new[]
            {
                new Option("", "No preference"),
                new Option("h264", "H.264"),
                new Option("hevc", "HEVC"),
                new Option("vc1", "VC-1"),
                new Option("mpeg2video", "MPEG-2"),
                new Option("mpeg4", "MPEG-4"),
                new Option("wmv3", "WMV3"),
                new Option("wmv2", "WMV2"),
                new Option("vp9", "VP9"),
                new Option("msmpeg4", "MS-MPEG4 V1"),
                new Option("msmpeg4v2", "MS-MPEG4 V1"),
                new Option("msmpeg4v3", "MS-MPEG4 V3"),
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
        await _customDbContext.Database.CloseConnectionAsync();
        bool reset = await _customDbContext.Database.EnsureDeletedAsync();
        await _customDbContext.Database.MigrateAsync();
        DbInitializer.Initialize(_customDbContext);
        return reset;
    }
}