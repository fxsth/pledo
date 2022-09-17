using Web.Models;

namespace Web.Data;

public static class DbInitializer
{
    public static void Initialize(DbContext context)
    {
        if (!context.Settings.Any())
        {
            var movieDirectory = new Setting()
            {
                Key = "MovieDirectoryPath",
                Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            };
            var episodeDirectory = new Setting()
            {
                Key = "EpisodeDirectoryPath", Value = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
            };
            context.Settings.Add(movieDirectory);
            context.Settings.Add(episodeDirectory);
            context.SaveChanges();
        }
    }
}