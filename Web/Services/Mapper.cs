using System.Collections;
using Plex.ServerApi.PlexModels.Media;
using Web.Models;

namespace Web.Services;

public static class Mapper
{
    public static IEnumerable<T> GetFromMediaContainer<T>(MediaContainer mediaContainer, Library library,
        ILogger<PlexRestService> logger)
    {
        if (typeof(T) == typeof(Movie))
            return (IEnumerable<T>) GetMoviesFromMediaContainer(mediaContainer, library, logger);
        if (typeof(T) == typeof(Episode))
            return (IEnumerable<T>) GetEpisodesFromMediaContainer(mediaContainer, library, logger);
        throw new Exception();
    }

    public static IEnumerable<Movie> GetMoviesFromMediaContainer(MediaContainer mediaContainer, Library library,
        ILogger<PlexRestService> logger)
    {
        foreach (var metadata in mediaContainer.Media)
        {
            if (metadata.Media?.FirstOrDefault()?.Part?.Any() != true)
            {
                logger.LogWarning("Movie {0} will be skipped, because it does not contain any file.", metadata.Title);
                break;
            }

            if (metadata.Media.Count > 1 || metadata.Media.First().Part.Count > 1)
            {
                logger.LogTrace("Movie {0} contains more than one file.", metadata.Title);
            }

            List<MediaFile> mediaFiles = new List<MediaFile>();
            foreach (var medium in metadata.Media)
            {
                foreach (var part in medium.Part)
                {
                    var mediaFile = Map(medium, part, library, metadata);
                    mediaFile.MovieRatingKey = metadata.RatingKey;
                    mediaFiles.Add(mediaFile);
                }
            }

            yield return new Movie()
            {
                Title = metadata.Title,
                Key = metadata.Key,
                RatingKey = metadata.RatingKey,
                LibraryId = library.Id,
                ServerId = library.Server.Id,
                Year = metadata.Year,
                MediaFiles = mediaFiles
            };
        }
    }

    public static IEnumerable<Episode> GetEpisodesFromMediaContainer(MediaContainer mediaContainer, Library library,
        ILogger<PlexRestService> logger)
    {
        foreach (var metadata in mediaContainer.Media)
        {
            if (metadata.Media?.FirstOrDefault()?.Part?.Any() != true)
            {
                logger.LogWarning("Episode {0} will be skipped, because file is missing.", metadata.Title);
                break;
            }

            if (metadata.Media.Count > 1 || metadata.Media.First().Part.Count > 1)
            {
                logger.LogTrace(
                    "Episode {0} contains more than one file, this program does not support more than one file",
                    metadata.Title);
            }

            List<MediaFile> mediaFiles = new List<MediaFile>();
            foreach (var medium in metadata.Media)
            {
                foreach (var part in medium.Part)
                {
                    var mediaFile = Map(medium, part, library, metadata);
                    mediaFile.EpisodeRatingKey = metadata.RatingKey;
                    mediaFiles.Add(mediaFile);
                }
            }

            yield return new Episode()
            {
                Title = metadata.Title,
                Key = metadata.Key,
                RatingKey = metadata.RatingKey,
                LibraryId = library.Id,
                ServerId = library.ServerId,
                Year = metadata.Year,
                MediaFiles = mediaFiles,
                EpisodeNumber = metadata.Index,
                SeasonNumber = metadata.ParentIndex,
                TvShowId = metadata.GrandparentRatingKey,
            };
        }
    }

    private static MediaFile Map(Medium medium, MediaPart part, Library library, Metadata x)
    {
        return new MediaFile()
        {
            Key = x.Key,
            RatingKey = x.RatingKey,
            ServerFilePath = part.File,
            DownloadUri = part.Key,
            LibraryId = library.Id,
            ServerId = library.Server.Id,
            TotalBytes = part.Size,
            Bitrate = medium.Bitrate,
            Container = medium.Container,
            Duration = medium.Duration,
            Height = medium.Height,
            Width = medium.Width,
            AspectRatio = medium.AspectRatio,
            AudioChannels = medium.AudioChannels,
            AudioCodec = medium.AudioCodec,
            AudioProfile = medium.AudioProfile,
            VideoCodec = medium.VideoCodec,
            VideoProfile = medium.VideoProfile,
            VideoResolution = medium.VideoResolution,
            VideoFrameRate = medium.VideoFrameRate
        };
    }
}