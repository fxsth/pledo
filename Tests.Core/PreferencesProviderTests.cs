using Web.Services;

namespace Tests.Core;

public class PreferencesProviderTests
{
    [Fact]
    public void GetFilenameFromPath_WindowsPath_ReturnsFilename()
    {
        string filepath = @"Y:\Movies\Title (2023).mkv";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("Title (2023).mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_LinuxPath_ReturnsFilename()
    {
        string filepath = @"/media/movies/Title (2023).mkv";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("Title (2023).mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_PathWithSpecialChar_ReturnsFilename()
    {
        string filepath = @"/media/mövüs/Tìtle (2023).mkv";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("Tìtle (2023).mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_PathWithMultipleBackSlashes_ReturnsFilename()
    {
        string filepath = @"Y:\\Movies\\Title (2023).mkv";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("Title (2023).mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_PathWithMultipleSlashes_ReturnsFilename()
    {
        string filepath = @"media//Movies//Ti_tle (2023).mkv";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("Ti_tle (2023).mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_EmptyPath_ReturnsBackupFilename()
    {
        string filepath = "";
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("file.mkv", filename);
    }
    
    [Fact]
    public void GetFilenameFromPath_Null_ReturnsBackupFilename()
    {
        string filepath = null!;
        string filename = PreferencesProvider.GetFilenameFromPath(filepath, "file.mkv");
        
        Assert.Equal("file.mkv", filename);
    }
}