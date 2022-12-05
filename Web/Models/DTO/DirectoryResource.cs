namespace Web.Models.DTO;

public class DirectoryResource
{
    public string CurrentDirectory { get; set; }
    public IEnumerable<string> SubDirectories { get; set; }
}