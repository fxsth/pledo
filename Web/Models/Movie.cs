using System.ComponentModel.DataAnnotations;
using Web.Models.Interfaces;

namespace Web.Models;

public class Movie : IMediaElement
{
    [Key]
    public string RatingKey { get; set; }
    public string Key { get; set; }
    public string Title { get; set; }
    public string ServerFilePath { get; set; }
    public string DownloadUri { get; set; }
    public long TotalBytes { get; set; }
    public string LibraryId { get; set; }
    public string ServerId { get; set; }
}