using Web.Models;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(DbContext context)
    {
        if (!context.Settings.Any())
        {
            var movieDirectory = new KeyValueSetting()
            {
                Key = "MovieDirectoryPath",
                Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Name = "Download directory for movies",
                Type = "path"
            };
            var episodeDirectory = new KeyValueSetting()
            {
                Key = "EpisodeDirectoryPath", 
                Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Name = "Download directory for tv-shows",
                Type = "path"
            };
            context.Settings.Add(movieDirectory);
            context.Settings.Add(episodeDirectory);
            context.SaveChanges();
        }
    }
}