using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web.Models;

public class Episode
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
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    [ForeignKey("TvShow")]
    public string TvShowId { get; set; }
    public TvShow TvShow { get; set; }
}