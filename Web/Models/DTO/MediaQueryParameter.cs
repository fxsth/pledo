namespace Web.Models.DTO;

public class MediaQueryParameter
{
    public string LibraryId { get; set; }
    public string? MediaType { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; } = 100;
}