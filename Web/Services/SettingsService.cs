using Web.Data;
using Web.Models.DTO;

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

    public async Task<string> GetMovieDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(MovieDirectoryKey);
        return setting.Value;
    }
    
    public async Task<string> GetEpisodeDirectory()
    {
        var setting = await _unitOfWork.SettingRepository.GetById(EpisodeDirectoryKey);
        return setting.Value;
    }

    public async Task<IEnumerable<SettingsResource>> GetSettings()
    {
        var settings = _unitOfWork.SettingRepository.GetAll();
        return settings.Select(x => new SettingsResource()
        {
            Key = x.Key,
            Description = x.Description,
            Value = x.Value,
            Name = x.Name
        });
    }

    public async Task UpdateSettings(IEnumerable<SettingsResource> settings)
    {
        var movieDirectory = await _unitOfWork.SettingRepository.GetById(MovieDirectoryKey);
        var episodeDirectory = await _unitOfWork.SettingRepository.GetById(EpisodeDirectoryKey);
        movieDirectory.Value = settings.FirstOrDefault(x=>x.Key==MovieDirectoryKey).Value;
        episodeDirectory.Value = settings.FirstOrDefault(x=>x.Key==EpisodeDirectoryKey).Value;
        await _unitOfWork.Save();
    }
}