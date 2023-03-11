namespace Web.Models.DTO;

public class PlaylistResource
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<PlaylistItem> Items { get; set; }
    public string ServerId { get; set; }
    public Server Server { get; set; }
}

public class PlaylistItem
{
    public string Id { get; set; }
    public string Name { get; set; }
    public ElementType? Type { get; set; }
}