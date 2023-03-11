using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Web.Models.Interfaces;

namespace Web.Models;

public class Episode : IMediaElement
{
    [Key] public string RatingKey { get; set; }
    public string Key { get; set; }
    public string Title { get; set; }
    public string ServerFilePath { get; set; }
    public string DownloadUri { get; set; }
    public long TotalBytes { get; set; }
    public string LibraryId { get; set; }
    public string ServerId { get; set; }
    public int? Year { get; set; }
    public long? Duration { get; set; }
    public long? Bitrate { get; set; }
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? AspectRatio { get; set; }
    public int? AudioChannels { get; set; }
    public string? AudioCodec { get; set; }
    public string? VideoCodec { get; set; }
    public string? VideoResolution { get; set; }
    public string? Container { get; set; }
    public string? VideoFrameRate { get; set; }
    public string? AudioProfile { get; set; }
    public string? VideoProfile { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    [ForeignKey("TvShow")]
    public string TvShowId { get; set; }
    [JsonIgnore]
    public TvShow TvShow { get; set; }
}