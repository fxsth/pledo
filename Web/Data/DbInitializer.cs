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
                Name = "Movie Download Directory"
            };
            var episodeDirectory = new KeyValueSetting()
            {
                Key = "EpisodeDirectoryPath", 
                Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                Name = "Movie Download Directory"
            };
            context.Settings.Add(movieDirectory);
            context.Settings.Add(episodeDirectory);
            context.SaveChanges();
        }
    }
}