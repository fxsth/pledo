namespace Web.Services;

public static class PreferencesProvider
{
    private const string ClientIdFilename = "clientInformation.json";

    public static string GetDefaultMovieDirectory()
    {
        if(OperatingSystem.IsWindows())
            return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        else if (Directory.Exists("/movies"))
            return "/movies";
        else if (Directory.Exists("/films"))
            return "/films";
        else if (Directory.Exists("/media"))
            return "/media";
        else
            return "";
    }
    
    public static string GetDefaultTvShowDirectory()
    {
        if(OperatingSystem.IsWindows())
            return Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
        else if (Directory.Exists("/tvshows"))
            return "/tvshows";
        else if (Directory.Exists("/series"))
            return "/series";
        else if (Directory.Exists("/media"))
            return "/media";
        else
            return "";
    }

    public static string GetDataDirectory()
    {
        try
        {
            if (Directory.Exists("config"))
                return "/config/";
            
            string dataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                ".pledo");
            Directory.CreateDirectory(dataDirectory);
            return dataDirectory;
        }
        catch (Exception)
        {
            return "";
        }
    }
    public static string GetClientId()
    {
        try
        {
            string path = Path.Combine(GetDataDirectory(), ClientIdFilename);
            if (File.Exists(path))
                return File.ReadAllText(path);
            var newClientId = Guid.NewGuid().ToString();
            File.WriteAllText(path, newClientId);
            return newClientId;
        }
        catch (Exception)
        {
            return Guid.NewGuid().ToString();
        }
    }
}