using Web.Constants;
using Web.Models;
using Web.Services;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(DbContext context)
    {
        context.AddSettingIfNotExist(new Setting()
        {
            Key = SettingsConstants.MovieDirectoryKey,
            Value = PreferencesProvider.GetDefaultMovieDirectory(),
            Name = "Download directory for movies",
            Type = "path"
        });
        context.AddSettingIfNotExist(new Setting()
        {
            Key = SettingsConstants.EpisodeDirectoryKey,
            Value = PreferencesProvider.GetDefaultTvShowDirectory(),
            Name = "Download directory for tv-shows",
            Type = "path"
        });
        context.AddSettingIfNotExist(new Setting()
        {
            Key = SettingsConstants.MovieFileTemplateKey,
            Value = MovieFileTemplate.FilenameFromServer.ToString(),
            Name = "Movie file template",
            Type = "enum"
        });
        context.AddSettingIfNotExist(new Setting()
        {
            Key = SettingsConstants.EpisodeFileTemplateKey,
            Value = EpisodeFileTemplate.SeriesAndSeasonDirectoriesAndFilenameFromServer.ToString(),
            Name = "Episode file template",
            Type = "enum"
        });
        context.SaveChanges();
    }

    private static bool AddSettingIfNotExist(this DbContext context, Setting setting)
    {
        if (context.Settings.Any() && context.Settings.Find(setting.Key) != null) return false;
        context.Settings.Add(setting);
        return true;
    }
}