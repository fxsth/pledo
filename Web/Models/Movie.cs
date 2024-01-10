using System.ComponentModel.DataAnnotations;
using Web.Models.Interfaces;

namespace Web.Models;

public class Movie : IMediaElement, ISearchable
{
    [Key]
    public string RatingKey { get; set; }
    public string Key { get; set; }
    public string Title { get; set; }
    public string LibraryId { get; set; }
    public string ServerId { get; set; }
    public int? Year { get; set; }

    public List<MediaFile> MediaFiles { get; set; } = new();
}