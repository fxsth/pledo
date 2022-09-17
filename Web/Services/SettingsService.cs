using Web.Data;
using Web.Models;

namespace Web.Services;

public class SettingsService : ISettingsService
{
    private const string MovieDirectoryKey = "MovieDirectoryPath";
    private const string EpisodeDirectoryKey = "EpisodeDirectoryPath";
    private readonly UnitOfWork _unitOfWork;

    public SettingsService(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Directories> GetDirectories()
    {
        var movieDirectory = await _unitOfWork.SettingRepository.GetById(MovieDirectoryKey);
        var episodeDirectory = await _unitOfWork.SettingRepository.GetById(EpisodeDirectoryKey);
        return new Directories()
            { MovieDownloadPath = movieDirectory.Value, EpisodeDownloadPath = episodeDirectory.Value };
    }

    public async Task UpdateDirectories(Directories directories)
    {
        var movieDirectory = await _unitOfWork.SettingRepository.GetById(MovieDirectoryKey);
        var episodeDirectory = await _unitOfWork.SettingRepository.GetById(EpisodeDirectoryKey);
        movieDirectory.Value = directories.MovieDownloadPath;
        episodeDirectory.Value = directories.EpisodeDownloadPath;
        await _unitOfWork.Save();
    }
}