using Web.Models;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(DbContext context)
    {
        context.AddSettingIfNotExist(new KeyValueSetting()
        {
            Key = "MovieDirectoryPath",
            Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            Name = "Download directory for movies",
            Type = "path"
        });
        context.AddSettingIfNotExist(new KeyValueSetting()
        {
            Key = "EpisodeDirectoryPath",
            Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            Name = "Download directory for tv-shows",
            Type = "path"
        });
        context.AddSettingIfNotExist(new KeyValueSetting()
        {
            Key = "MovieFileTemplate",
            Value = MovieFileTemplate.FilenameFromServer.ToString(),
            Name = "Movie file template",
            Type = "enum"
        });
        context.AddSettingIfNotExist(new KeyValueSetting()
        {
            Key = "EpisodeFileTemplate",
            Value = EpisodeFileTemplate.SeriesAndSeasonDirectoriesAndFilenameFromServer.ToString(),
            Name = "Episode file template",
            Type = "enum"
        });
        context.SaveChanges();
    }

    private static bool AddSettingIfNotExist(this DbContext context, KeyValueSetting setting)
    {
        if (context.Settings.Any() && context.Settings.Find(setting.Key) != null) return false;
        context.Settings.Add(setting);
        return true;
    }
}