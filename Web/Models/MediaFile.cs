using Microsoft.EntityFrameworkCore;

namespace Web.Models;

[PrimaryKey(nameof(DownloadUri), nameof(ServerId))]
public class MediaFile
{
    public string RatingKey { get; set; }
    public string Key { get; set; }
    public string ServerFilePath { get; set; }
    
    public string? MovieRatingKey { get; set; }
    public string? EpisodeRatingKey { get; set; }
    
    public string DownloadUri { get; set; }
    public long TotalBytes { get; set; }
    public string LibraryId { get; set; }
    public string ServerId { get; set; }
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
}