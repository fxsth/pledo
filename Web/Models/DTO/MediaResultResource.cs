using Web.Models.Interfaces;

namespace Web.Models.DTO;

public class MediaResultResource
{
    public int TotalItems { get; set; }
    public IEnumerable<IMediaElement> Items { get; set; }
}