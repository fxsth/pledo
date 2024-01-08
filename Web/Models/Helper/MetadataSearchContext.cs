namespace Web.Models.Helper;

public class MetadataSearchContext
{
    public string ServerName { get; set; }
    public string LibraryName { get; set; }
    public string LibraryId { get; set; }
    public int Offset { get; set; }
    public int BatchSize { get; set; }
    public ElementType ElementType { get; set; }
}